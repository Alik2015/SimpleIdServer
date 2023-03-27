﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class BCConsentsIndexViewModel
    {
        public string AuthReqId { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string BindingMessage { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public ICollection<AuthorizationData> AuthorizationDetails {  get; set; }
        public bool IsConfirmed { get; set; }
        public ConfirmationStatus ConfirmationStatus{ get; set; }
    }

    public enum ConfirmationStatus
    {
        REJECTED = 0,
        CONFIRMED = 1
    }
}
