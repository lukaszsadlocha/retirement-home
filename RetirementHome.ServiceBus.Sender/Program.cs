using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace RetirementHome.ServiceBus.Sender
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

            // Sending to Azure ServiceBus
            var sender = client.CreateSender(queueName);

            var stingToSent = $"This message was prepared at: {DateTime.UtcNow}";

            var message = new ServiceBusMessage(stingToSent);

            Console.WriteLine("Sending...");
            await sender.SendMessageAsync(message, CancellationToken.None);

            await sender.CloseAsync();

            Console.WriteLine("Sent\n");


            // Receiving
            var receiver = client.CreateReceiver(queueName);

            var cts = new CancellationTokenSource(10000);
            var cancellationToken = cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                var receivedMessage = await receiver.ReceiveMessageAsync();

                if(receivedMessage != null)
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

