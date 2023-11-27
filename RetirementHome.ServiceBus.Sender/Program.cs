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
            
            var sender = client.CreateSender(queueName);

            var stingToSent = $"This message was prepared at: {DateTime.UtcNow}";

            var message = new ServiceBusMessage(stingToSent);

            Console.WriteLine("Sending...");
            await sender.SendMessageAsync(message, CancellationToken.None);

            await sender.CloseAsync();

            Console.WriteLine("Sent\n");

        }
    }
}

