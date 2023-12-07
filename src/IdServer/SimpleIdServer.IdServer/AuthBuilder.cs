﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Options;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer
{
    public class AuthBuilder
    {
        private readonly IServiceCollection _services;
        private readonly AuthenticationBuilder _authBuilder;

        internal AuthBuilder(IServiceCollection services, AuthenticationBuilder authBuilder)
        {
            _services = services;
            _authBuilder = authBuilder;
        }

        public AuthBuilder AddMutualAuthentication(Action<CertificateAuthenticationOptions> callback = null)
        {
            _services.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
            });
            _authBuilder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, callback != null ? callback : o =>
            {

            });
            return this;
        }

        public AuthBuilder AddMutualAuthenticationSelfSigned()
        {
            _services.Configure<IdServerHostOptions>(o =>
            {
                o.MtlsEnabled = true;
            });
            _authBuilder.AddCertificate(Constants.DefaultCertificateAuthenticationScheme, o =>
            {
                o.AllowedCertificateTypes = CertificateTypes.SelfSigned;
            });
            return this;
        }

        public AuthenticationBuilder Builder => _authBuilder;
    }
}