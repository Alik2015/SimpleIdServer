﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class TokenResponseParameters
    {
        public const string AccessToken = "access_token";
        public const string TokenType = "token_type";
        public const string RefreshToken = "refresh_token";
        public const string ExpiresIn = "expires_in";
        public const string Scope = "scope";
        public const string IdToken = "id_token";
        public const string GrantId = "grant_id";
        public const string CNonce = "c_nonce";
        public const string CNonceExpiresIn = "c_nonce_expires_in";
    }
}