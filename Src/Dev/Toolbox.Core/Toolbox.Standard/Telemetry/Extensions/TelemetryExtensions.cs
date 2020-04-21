// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class TelemetryExtensions
    {
        public static IWorkContext WithCreateLogger(this IWorkContext context, string eventSourceName)
        {
            context.VerifyNotNull(nameof(context));
            eventSourceName.VerifyNotNull(nameof(eventSourceName));

            if (context.Container == null) return context;

            ITelemetry telementry = context.Container.Resolve<ITelemetryService>()
                 .CreateLogger(eventSourceName);

            return context.With(telementry);
        }

        public static IEventDimensions ToEventDimensions(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return new EventDimensions(keyValuePairs);
        }

        public static IEventDimensions ToEventDimensions(this object dimensions)
        {
            dimensions.VerifyNotNull(nameof(dimensions));

            return dimensions.ToKeyValues().ToEventDimensions();
        }

        public static ITelemetrySecretManager BuildSecretManager(this object optionClass)
        {
            optionClass.VerifyNotNull(nameof(optionClass));

            IReadOnlyList<PropertyPathValue> properties = optionClass.ToKeyValuesForAttribute<TelemetrySecretAttribute>();

            return new TelemetrySecretManager().Add(properties.Select(x => x.Value.ToString()).ToArray());
        }
    }
}
