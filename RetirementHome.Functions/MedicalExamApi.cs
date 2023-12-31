using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.WindowsAzure.Storage;

namespace RetirementHome.Functions
{
    public static class MedicalExamApi
    {
        [FunctionName("CreateMedicalExam")]
        public static async Task<IActionResult> CreateMedicalExam(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "medicalexam")] HttpRequest req,
            [Table("MedicalExams", Connection = "AzureWebJobsStorage")] IAsyncCollector<MedicalExamTableEntity> medicalExamTable,
            ILogger log)
        {
            log.LogInformation("Creating new MedicalExam.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<MedicalExamCreateModel>(requestBody);

            var medicalExam = new MedicalExam { Description = input.Description };

            await medicalExamTable.AddAsync(medicalExam.ToTableEntity());

            return new OkObjectResult(medicalExam);
        }

        [FunctionName("GetMedicalExams")]
        public static async Task<IActionResult> GetMedicalExams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "medicalexam")] HttpRequest req,
            [Table(tableName:"MedicalExams", partitionKey: "MEDICAL_EXAM", Connection = "AzureWebJobsStorage", Take = 10)] TableClient medicalExamsTableClient,
            ILogger log)
        {
            log.LogInformation("Getting MedicalExam list");

            var queryResultsMaxPerPage = medicalExamsTableClient.QueryAsync<MedicalExamTableEntity>();

            var medicalExamList = new List<MedicalExam>();
            await foreach (var page in queryResultsMaxPerPage.AsPages())
            {
                medicalExamList.AddRange(page.Values.Select(exam => exam.ToMedicalExam()));
            }

            return new OkObjectResult(medicalExamList);
        }

        [FunctionName("GetMedicalExamById")]
        public static async Task<IActionResult> GetMedicalExamById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "medicalexam/{id}")] HttpRequest req,
            [Table("MedicalExams", partitionKey: "MEDICAL_EXAM", rowKey: "{id}", Connection = "AzureWebJobsStorage")] MedicalExamTableEntity exam,
            ILogger log, string id)
        {
            log.LogInformation("Getting MedicalExam by id = {id}", id);

            if (exam == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(exam.ToMedicalExam());
        }

        [FunctionName("UpdateMedicalExamById")]
        public static async Task<IActionResult> UpdateMedicalExamById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "medicalexam/{id}")] HttpRequest req,
            [Table("MedicalExams", Connection = "AzureWebJobsStorage")] TableClient medicalExamsTableClient,
            ILogger log, string id)
        {
            log.LogInformation("Updating MedicalExam by id = {id}", id);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<MedicalExamUpdateModel>(requestBody);

            var findResult = await medicalExamsTableClient.GetEntityIfExistsAsync<MedicalExamTableEntity>("MEDICAL_EXAM", id);

            if (!findResult.HasValue)
            {
                return new NotFoundResult();
            }

            var existingRow = findResult.Value;

            existingRow.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.Description))
            {
                existingRow.Description = updated.Description;
            }

            var replaceOperation = await medicalExamsTableClient.UpdateEntityAsync<MedicalExamTableEntity>(existingRow, ETag.All);
            
            return new OkObjectResult(existingRow.ToMedicalExam());
        }

        [FunctionName("DeleteMedicalExamById")]
        public static async Task<IActionResult> DeleteMedicalExamById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "medicalexam/{id}")] HttpRequest req,
            [Table("MedicalExams", Connection = "AzureWebJobsStorage")] TableClient medicalExamsTableClient,
            ILogger log, string id)
        {
            log.LogInformation("Deleting MedicalExam by id = {id}", id);

            try
            {
                var deleteResult = await medicalExamsTableClient.DeleteEntityAsync("MEDICAL_EXAM", id);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }
    }

    public class MedicalExam
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class MedicalExamCreateModel
    {
        public string Description { get; set; }
    }

    public class MedicalExamUpdateModel
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    //Entity for Table storage in Azure;
    public class MedicalExamTableEntity : Azure.Data.Tables.ITableEntity
    {

        public DateTime CreatedTime { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public static class Mappings
    {
        public static MedicalExamTableEntity ToTableEntity(this MedicalExam medicalExam)
        {
            return new MedicalExamTableEntity
            {
                PartitionKey = "MEDICAL_EXAM",
                RowKey = medicalExam.Id,
                CreatedTime = medicalExam.CreatedTime,
                Description = medicalExam.Description,
                IsCompleted = medicalExam.IsCompleted

            };
        }

        public static MedicalExam ToMedicalExam(this MedicalExamTableEntity medicalExam)
        {
            return new MedicalExam
            {
                Id = medicalExam.RowKey,
                CreatedTime = medicalExam.CreatedTime,
                IsCompleted = medicalExam.IsCompleted,
                Description = medicalExam.Description
            };
        }
    }
}