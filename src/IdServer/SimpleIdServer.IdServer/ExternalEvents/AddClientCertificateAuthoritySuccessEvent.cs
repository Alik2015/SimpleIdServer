﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.IdServer.ExternalEvents;

public class AddClientCertificateAuthoritySuccessEvent : IExternalEvent
{
    public string EventName => nameof(AddClientCertificateAuthoritySuccessEvent);
    public string Realm { get; set; }
    public string CAId { get; set; }
    public string SubjectName { get; set; }
    public int NbDays { get; set; }
}
