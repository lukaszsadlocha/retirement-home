using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RetirementHome.Connectors
{
    public class FitDataFetcher
    {
        public const string queueName = "healthdata";
        public const string serviceBusConnection = "ServiceBusConnection";
        
        [FunctionName(nameof(FitDataFetcher))]
        [return: ServiceBus(queueName, Connection = serviceBusConnection)]
        public static string ServiceBusOutput(
            [TimerTrigger("0 5 * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation("Next occurence {nextOccurenct}", myTimer.FormatNextOccurrences(1));

            return $"This is a message from AzureFunction - at {DateTime.Now}";

            //TODO - add extra parameters of who this data is about
        }
    }
}

