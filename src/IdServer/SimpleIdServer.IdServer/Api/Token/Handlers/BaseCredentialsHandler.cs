﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public abstract class BaseCredentialsHandler : IGrantTypeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IdServerHostOptions _options;

        public BaseCredentialsHandler(IClientAuthenticationHelper clientAuthenticationHelper, IGrantedTokenHelper grantedTokenHelper, IOptions<IdServerHostOptions> options)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _grantedTokenHelper = grantedTokenHelper;
            _options = options.Value;
        }

        public abstract string GrantType { get; }
        public abstract Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);
        protected IdServerHostOptions Options => _options;
        protected IGrantedTokenHelper GrantedTokenHelper => _grantedTokenHelper;

        protected async Task<Client> AuthenticateClient(HandlerContext context, CancellationToken cancellationToken)
        {
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.GetIssuer(), cancellationToken);
            if (oauthClient.GrantTypes == null || !oauthClient.GrantTypes.Contains(GrantType)) throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.BAD_CLIENT_GRANT_TYPE, GrantType));
            return oauthClient;
        }

        protected bool TryGetClientId(HandlerContext context, out string clientId) => _clientAuthenticationHelper.TryGetClientId(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, out clientId);

        protected IEnumerable<string> GetScopes(JsonObject previousRequest, HandlerContext context) => GetScopes(previousRequest, context.Request.RequestData);

        protected IEnumerable<string> GetScopes(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetScopesFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetScopesFromAuthorizationRequest();
            return result;
        }

        protected IEnumerable<string> GetResources(JsonObject previousRequest, HandlerContext context) => GetResources(previousRequest, context.Request.RequestData);

        protected IEnumerable<string> GetResources(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetResourcesFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetResourcesFromAuthorizationRequest();
            return result;
        }

        protected IEnumerable<AuthorizedClaim> GetClaims(JsonObject previousRequest, HandlerContext context) => GetClaims(previousRequest, context.Request.RequestData);

        protected IEnumerable<AuthorizedClaim> GetClaims(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetClaimsFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetClaimsFromAuthorizationRequest();
            return result;
        }

        protected ICollection<AuthorizationData> GetAuthorizationDetails(JsonObject previousRequest, JsonObject newRequest)
        {
            var result = newRequest.GetAuthorizationDetailsFromAuthorizationRequest();
            if ((result == null || !result.Any()) && previousRequest != null)
                result = previousRequest.GetAuthorizationDetailsFromAuthorizationRequest();
            return result;
        }

        protected async Task AddCredentialParameters(HandlerContext context, JsonObject result, CancellationToken cancellationToken)
        {
            var expiresIn = context.Client.CNonceExpirationTimeInSeconds ?? _options.DefaultCNonceExpirationTimeInSeconds.Value;
            var credentialNonce = Guid.NewGuid().ToString();
            await _grantedTokenHelper.AddCredentialNonce(credentialNonce, expiresIn, cancellationToken);
            result.Add(TokenResponseParameters.CNonce, credentialNonce);
            result.Add(TokenResponseParameters.CNonceExpiresIn, expiresIn);
        }

        protected JsonObject BuildResult(HandlerContext context, IEnumerable<string> scopes)
        {
            return new JsonObject
            {
                [TokenResponseParameters.ExpiresIn] = context.Client.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds,
                [TokenResponseParameters.Scope] = string.Join(" ", scopes)
            };
        }

        public static IActionResult BuildError(HttpStatusCode httpStatusCode, string error, string errorMessage)
        {
            var jObj = new JsonObject
            {
                [ErrorResponseParameters.Error] = error,
                [ErrorResponseParameters.ErrorDescription] = errorMessage
            };
            return new ContentResult
            {
                ContentType = "application/json",
                Content = jObj.ToString(),
                StatusCode = (int)httpStatusCode
            };
        }
    }
}