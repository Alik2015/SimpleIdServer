﻿// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class NetworkConfiguration
    {
        public string Name { get; set; }
        public string RpcUrl { get; set; }
        public string PrivateAccountKey { get; set; }
        public string ContractAdr { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
