using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RetirementHome.ServiceBus.Sender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Configuration.AddUserSecrets<Program>();
            builder.Services.AddHostedService<ServiceBusSenderHostedService>();
            builder.Services.AddHttpClient<IServiceBusSenderExample, ServiceBusSenderExample>(c=>
                c.BaseAddress = new Uri(@"https://api.ciuchapp.lukaszsadlocha.pl"));
            builder.Build().Run();
        }
    }
    internal class ServiceBusSenderHostedService : IHostedService
    {
        public ServiceBusSenderHostedService(IServiceBusSenderExample serviceBusSenderExample)
        {
            ServiceBusSenderExample = serviceBusSenderExample;
        }
        public IServiceBusSenderExample ServiceBusSenderExample { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            await ServiceBusSenderExample.Run();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IServiceBusSenderExample
    {
        Task Run();
    }

    internal class ServiceBusSenderExample : IServiceBusSenderExample
    {
        public ServiceBusSenderExample(IConfiguration configuration, HttpClient httpClient)
        {
            Configuration = configuration;
            HttpClient = httpClient;
        }

        public IConfiguration Configuration { get; }
        public HttpClient HttpClient { get; }

        public async Task Run()
        {
            var response = await HttpClient.GetAsync("/Pieces");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();





            var azServiceBusConnString = Configuration.GetConnectionString("AzureServiceBus");
            var queueName = Configuration.GetSection("queueName").Value;

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

