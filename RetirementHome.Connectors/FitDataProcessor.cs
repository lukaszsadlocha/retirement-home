using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RetirementHome.Connectors
{
    public class FitDataProcessor
    {
        public const string queueName = "healthdata";
        public const string serviceBusConnection = "ServiceBusConnection";

        [FunctionName(nameof(FitDataFetcher))]
        public void Run([ServiceBusTrigger("healthData", Connection = "ServiceBys")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
        }
    }
}
