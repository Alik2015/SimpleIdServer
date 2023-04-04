﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class Realm
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<RealmUser> Users { get; set; } = new List<RealmUser>();
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
        public ICollection<AuthenticationContextClassReference> AuthenticationContextClassReferences { get; set; } = new List<AuthenticationContextClassReference>();
        public ICollection<AuthenticationSchemeProvider> AuthenticationSchemeProviders { get; set; } = new List<AuthenticationSchemeProvider>();
        public ICollection<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public ICollection<SerializedFileKey> SerializedFileKeys { get; set; } = new List<SerializedFileKey>();
        public ICollection<CertificateAuthority> CertificateAuthorities { get; set; } = new List<CertificateAuthority>();
        public ICollection<IdentityProvisioning> IdentityProvisioningLst { get; set; } = new List<IdentityProvisioning>();
        public ICollection<ImportSummary> ImportSummaries { get; set; } = new List<ImportSummary>();
    }
}