using Google.Apis.Fitness.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace RetirementHome.FitDataConnector;

internal class FitDataWorker
{
    private readonly IConfiguration configuration;

    public FitDataWorker(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task DoWork()
    {
        var apiKey = configuration["GoogleApiKey"] ?? 
            throw new ConfigurationErrorsException("GoogleApiKey configuraiton is missing");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ConfigurationErrorsException("GoogleApiKey configuraiton is empty");
        }

        await DiscoveryApiExample.Run(apiKey);

        var clientServiceInitializer = new BaseClientService.Initializer
        {
            ApplicationName = "Retirement home",
            ApiKey = apiKey
        };

        var service = new FitnessService(clientServiceInitializer);
        // var feature = service.Features; //string[0]

        var a = service.Users.DataSources.Get("me", "");
    }
}
