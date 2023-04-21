﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public class UpdateUserState
    {
        public UpdateUserState() { }

        public UpdateUserState(bool isUpdating)
        {
            IsUpdating = isUpdating;
        }

        public bool IsUpdating { get; set; } = false;
        public string ErrorMessage { get; set; } = null;
    }
}
