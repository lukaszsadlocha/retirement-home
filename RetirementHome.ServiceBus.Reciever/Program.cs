using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace RetirementHome.ServiceBus.Reciever
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();


            var azServiceBusConnString = configuration.GetConnectionString("AzureServiceBus");
            var queueName = configuration.GetSection("queueName").Value;

            var client = new ServiceBusClient(azServiceBusConnString);

            // Receiving
            var receiver = client.CreateReceiver(queueName);

            var cts = new CancellationTokenSource(2000);
            var cancellationToken = cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"waiting for message...");
                var receivedMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

                if (receivedMessage != null)
                {
                    Console.WriteLine($"Received message: {receivedMessage.Body}");

                    //Complete the messae
                    await receiver.CompleteMessageAsync(receivedMessage);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("All messages received.");
                    break;
                }
            }

            // Close the receiver
            await receiver.CloseAsync();
        }
    }
}

