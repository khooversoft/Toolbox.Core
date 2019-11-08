using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    internal interface ISendEvent
    {
        Task SendAsync(IWorkContext context, EventData eventData);

        Task CloseAsync(IWorkContext context);
    }
}
