// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Azure
{
    public class QueueRegistration
    {

        public QueueRegistration(string nameSpace, QueueDefinition queueDefinition)
        {
            nameSpace.Verify(nameof(nameSpace)).IsNotEmpty();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            Namespace = nameSpace;
            QueueDefinition = queueDefinition;
        }

        public string Namespace { get; }

        public QueueDefinition QueueDefinition { get; }
    }
}
