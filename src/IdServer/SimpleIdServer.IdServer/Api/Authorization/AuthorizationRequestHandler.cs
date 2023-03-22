﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization
{
    public interface IAuthorizationRequestHandler
    {
        Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token);
    }

    public class AuthorizationRequestHandler : IAuthorizationRequestHandler
    {
        private readonly IAuthorizationRequestValidator _validator;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IAuthorizationRequestEnricher _authorizationRequestEnricher;
        private readonly IUserRepository _userRepository;
        private readonly IGrantRepository _grantRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IdServerHostOptions _options;

        public AuthorizationRequestHandler(IAuthorizationRequestValidator validator,
            IEnumerable<ITokenProfile> tokenProfiles, 
            IAuthorizationRequestEnricher authorizationRequestEnricher,
            IUserRepository userRepository,
            IGrantRepository grantRepository,
            ITokenRepository tokenRepository,
            IDistributedCache distributedCache,
            IOptions<IdServerHostOptions> options)
        {
            _validator = validator;
            _tokenProfiles = tokenProfiles;
            _authorizationRequestEnricher = authorizationRequestEnricher;
            _userRepository = userRepository;
            _grantRepository = grantRepository;
            _tokenRepository = tokenRepository;
            _distributedCache = distributedCache;
            _options = options.Value;
        }

        public virtual async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token)
        {
            try
            {
                var result = await BuildResponse(context, token);
                var display = context.Request.RequestData.GetDisplayFromAuthorizationRequest();
                if (!string.IsNullOrWhiteSpace(display))
                    context.Response.Add(AuthorizationRequestParameters.Display, display);
                var sessionState = BuildSessionState(context);
                if (!string.IsNullOrWhiteSpace(sessionState))
                    context.Response.Add(AuthorizationRequestParameters.SessionState, sessionState);
                return result;
            }
            catch (OAuthUserConsentRequiredException ex)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse(ex.ActionName, ex.ControllerName, context.Request.OriginalRequestData);
            }
            catch (OAuthLoginRequiredException ex)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Authenticate", context.Request.OriginalRequestData, ex.Area, true, new List<string> { _options.GetSessionCookieName() });
            }
            catch (OAuthSelectAccountRequiredException)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Accounts", context.Request.OriginalRequestData);
            }
        }

        protected async Task<AuthorizationResponse> BuildResponse(HandlerContext context, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAuthorizationRequest(context, cancellationToken);
            var user = await _userRepository.Query()
                .Include(u => u.Consents)
                .Include(u => u.Sessions)
                .Include(u => u.Realms)
                .Include(u => u.OAuthUserClaims)
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Name == context.Request.UserSubject && u.Realms.Any(r => r.Name == context.Realm), cancellationToken);
            context.SetUser(user);
            var grantRequest = validationResult.GrantRequest;
            var responseTypeHandlers = validationResult.ResponseTypes;
            await _validator.ValidateAuthorizationRequestWhenUserIsAuthenticated(grantRequest, context, cancellationToken);
            var state = context.Request.RequestData.GetStateFromAuthorizationRequest();
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(state))
                context.Response.Add(AuthorizationResponseParameters.State, state);

            _authorizationRequestEnricher.Enrich(context);
            var grant = await ExecuteGrantManagementAction(grantRequest, context, cancellationToken);
            foreach (var responseTypeHandler in responseTypeHandlers)
                await responseTypeHandler.Enrich(new EnrichParameter { Scopes = grantRequest.Scopes, Audiences = grantRequest.Audiences, GrantId = grant?.Id, Claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest() }, context, cancellationToken);

            _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
            return new RedirectURLAuthorizationResponse(redirectUri, context.Response.Parameters);
        }

        protected async Task<Grant> ExecuteGrantManagementAction(GrantRequest extractionResult, HandlerContext context, CancellationToken cancellationToken)
        {
            var grantId = context.Request.RequestData.GetGrantIdFromAuthorizationRequest();
            var grantManagementAction = context.Request.RequestData.GetGrantManagementActionFromAuthorizationRequest();
            if(string.IsNullOrWhiteSpace(grantManagementAction)) return null;
            var claims = context.Request.RequestData.GetClaimsFromAuthorizationRequest();
            var allClaims = claims.Select(c => c.Name).Distinct().ToList();
            Grant grant = null;
            switch (grantManagementAction)
            {
                // create a fresh grant.
                case Constants.StandardGrantManagementActions.Create:
                    {
                        grant = Grant.Create(context.Client.ClientId, allClaims, extractionResult.Authorizations);
                        _grantRepository.Add(grant);
                        await _grantRepository.SaveChanges(cancellationToken);
                    }
                    break;
                // change the grant to be ONLY the permissions requested by the client and consented by the resource owner.
                case Constants.StandardGrantManagementActions.Replace:
                    {
                        grant = await _grantRepository.Query().Include(g => g.Scopes).FirstOrDefaultAsync(g => g.Id == grantId);
                        if (grant == null)
                            throw new OAuthException(ErrorCodes.INVALID_GRANT, string.Format(ErrorMessages.UNKNOWN_GRANT, grantId));
                        if (grant.ClientId != context.Client.ClientId)
                            throw new OAuthException(ErrorCodes.ACCESS_DENIED, string.Format(ErrorMessages.UNAUTHORIZED_CLIENT_ACCESS_GRANT, context.Client.ClientId));

                        await RevokeTokens(grant.Id, cancellationToken);
                        grant.Claims = allClaims;
                        grant.Scopes = extractionResult.Authorizations;
                        await _grantRepository.SaveChanges(cancellationToken);
                    }
                    break;
                // merge the permissions consented by the resource owner in the actual request with those which already exist within the grant and shall invalidate existing refresh tokens associated with the updated grant
                case Constants.StandardGrantManagementActions.Merge:
                    {
                        grant = await _grantRepository.Query().Include(g => g.Scopes).FirstOrDefaultAsync(g => g.Id == grantId);
                        if (grant == null)
                            throw new OAuthException(ErrorCodes.INVALID_GRANT, string.Format(ErrorMessages.UNKNOWN_GRANT, grantId));
                        if (grant.ClientId != context.Client.ClientId)
                            throw new OAuthException(ErrorCodes.ACCESS_DENIED, string.Format(ErrorMessages.UNAUTHORIZED_CLIENT_ACCESS_GRANT, context.Client.ClientId));

                        grant.Merge(allClaims, extractionResult.Authorizations);
                        _grantRepository.Update(grant);
                        await _grantRepository.SaveChanges(cancellationToken);
                    }
                    break;
            }

            return grant;

            async Task RevokeTokens(string grantId, CancellationToken cancellationToken)
            {
                var tokens = await _tokenRepository.Query().Where(t => t.GrantId == grantId).ToListAsync(cancellationToken);
                foreach (var t in tokens)
                    _tokenRepository.Remove(t);
                await _tokenRepository.SaveChanges(cancellationToken);
            }
        }

        private string BuildSessionState(HandlerContext handlerContext)
        {
            var session = handlerContext.User.GetActiveSession(handlerContext.Realm ?? Constants.DefaultRealm);
            if (session == null)
                return null;

            var redirectUrl = handlerContext.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var clientId = handlerContext.Client.ClientId;
            var salt = Guid.NewGuid().ToString();
            return BuildSessionState(redirectUrl, clientId, salt, session.SessionId);
        }

        public static string BuildSessionState(string redirectUrl, string clientId, string salt, string sessionId)
        {
            var uri = new Uri(redirectUrl);
            var origin = uri.Scheme + "://" + uri.Host;
            if (!uri.IsDefaultPort)
            {
                origin += ":" + uri.Port;
            }

            var payload = Encoding.UTF8.GetBytes($"{clientId}{origin}{sessionId}{salt}");
            byte[] hash;
            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(payload);

            return hash.Base64EncodeBytes() + "." + salt;
        }
    }
}