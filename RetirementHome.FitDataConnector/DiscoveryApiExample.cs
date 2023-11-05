using System;
using System.Threading.Tasks;

using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;

namespace RetirementHome.FitDataConnector
{
    internal class DiscoveryApiExample
    {
        internal static async Task Run(string apiKey)
        {

            /// <summary>
            /// This example uses the discovery API to list all APIs in the discovery repository.
            /// https://developers.google.com/discovery/v1/using.
            /// <summary>

            Console.WriteLine("Discovery API Sample");
            Console.WriteLine("====================");

            // Create the service.
            var service = new DiscoveryService(new BaseClientService.Initializer
            {
                ApplicationName = "Retirement home",
                ApiKey = apiKey
            });

            // Run the request.
            Console.WriteLine("Executing a list request...");
            var result = await service.Apis.List().ExecuteAsync();

            // Display the results.
            if (result.Items != null)
            {
                var fitnessApi = result.Items.First(x => x.Title.Contains("Fitness"));

                Console.WriteLine(fitnessApi.Id + " - " + fitnessApi.Title);


            }
        }
    }
}