using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    public class SendEvents
    {
        private readonly IOption _option;

        public SendEvents(IOption option)
        {
            _option = option;
        }

        public async Task Run()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_option.EventHub.ConnectionString)
            {
                EntityPath = _option.EventHub.Name,
            };

            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(client);

            await client.CloseAsync();
        }

        private async Task SendMessagesToEventHub(EventHubClient client)
        {
            for (var i = 0; i < _option.Count && !_option.CancellationTokenSource.Token.IsCancellationRequested; i++)
            {
                try
                {
                    var message = $"Message {i} ***";
                    Console.WriteLine($"Sending message: {message}");
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{_option.Count} messages sent.");
        }
    }
}
