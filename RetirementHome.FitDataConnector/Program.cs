using Google.Apis.Discovery.v1;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RetirementHome.FitDataConnector;

/// <summary>
///     This example uses the discovery API to list all APIs in the discovery repository.
///     https://developers.google.com/discovery/v1/using.
///     <summary>
internal class Program
{
    [STAThread]
    private static async Task Main(string[] args)
    {
        try
        {
            // Setup Host
            var host = CreateDefaultBuilder().Build();

            // Invoke Worker
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<FitDataWorker>();
            await workerInstance.DoWork();

            host.Run();

        }
        catch (AggregateException ex)
        {
            foreach (var e in ex.InnerExceptions) Console.WriteLine("ERROR: " + e.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static IHostBuilder CreateDefaultBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("appsettings.json");
                //app.AddUserSecrets<FitDataWorker>(); <- not needed for now
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<FitDataWorker>();
            });
    }
}