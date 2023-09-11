﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationSchemeProviderDefinition
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string? Image { get; set; } = null;
        public string? HandlerFullQualifiedName { get; set; } = null;
        public string? OptionsFullQualifiedName { get; set; } = null;
        public string? OptionsName { get; set; } = null;
        public ICollection<AuthenticationSchemeProvider> AuthSchemeProviders { get; set; } = new List<AuthenticationSchemeProvider>();
    }
}
