using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Text;

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
        //var apiKey = configuration["GoogleApiKey"] ?? 
        //    throw new ConfigurationErrorsException("GoogleApiKey configuraiton is missing");
        //if (string.IsNullOrEmpty(apiKey))
        //{
        //    throw new ConfigurationErrorsException("GoogleApiKey configuraiton is empty");
        //}

        //await DiscoveryApiExample.Run(apiKey);

        UserCredential credential;
        using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] {
                    FitnessService.Scope.FitnessActivityRead,
                    FitnessService.Scope.FitnessBodyRead,
                    FitnessService.Scope.FitnessHeartRateRead,
                    FitnessService.Scope.FitnessSleepRead,
                    FitnessService.Scope.FitnessLocationRead,
                    FitnessService.Scope.FitnessBloodPressureRead,
                },
                "user", CancellationToken.None, new FileDataStore("FitData.MyData"));
        }

        // Create the service.
        var service = new FitnessService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "RetirementHome.FitDataConnector",
        });
        // Here service is authenticated

        var query = new EstimatedStepsQuery(service);
        var list = query.QueryStepPerDay(DateTime.Now.AddDays(-3), DateTime.Now);

        var dataTable = list
            .Select(w => new object[]
            {
        w.Stamp
            })
            .ToArray();
    }
}
