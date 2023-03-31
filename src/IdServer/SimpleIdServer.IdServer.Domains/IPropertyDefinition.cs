﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public interface IPropertyDefinition
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string? Description { get; set; }
    }
}
