// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    internal class EventSendClient : ISendEvent
    {
        private readonly EventHubClient _client;
        private readonly EventHubsConnectionStringBuilder _conntectionString;
        private static readonly StringVector _tag = new StringVector(nameof(EventSendClient));

        public EventSendClient(IOption option)
        {
            _conntectionString = new EventHubsConnectionStringBuilder(option.EventHub!.ConnectionString);
            _client = EventHubClient.CreateFromConnectionString(_conntectionString.ToString());
        }

        public Task CloseAsync(IWorkContext context)
        {
            context = context.With(_tag);

            context.Telemetry.Verbose(context, $"Closing event hub client for {_conntectionString.EntityPath}");
            return _client.CloseAsync();
        }

        public Task SendAsync(IWorkContext context, EventData eventData)
        {
            return _client.SendAsync(eventData);
        }
    }
}
