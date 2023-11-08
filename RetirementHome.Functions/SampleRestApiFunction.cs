using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace RetirementHome.Functions
{
    public static class MedicalExamApi
    {
        private static List<MedicalExam> exams = new();

        [FunctionName("CreateMedicalExam")]
        public static async Task<IActionResult> CreateMedicalExam(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "medicalexam")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating new MedicalExam.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<MedicalExamCreateModel>(requestBody);

            var medicalExam = new MedicalExam { Description = input.Description };

            exams.Add(medicalExam);

            return new OkObjectResult(medicalExam);
        }

        [FunctionName("GetMedicalExams")]
        public static async Task<IActionResult> GetMedicalExams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "medicalexam")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting MedicalExam list");

            return new OkObjectResult(exams);
        }

        [FunctionName("GetMedicalExamById")]
        public static async Task<IActionResult> GetMedicalExamById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "medicalexam/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("Getting MedicalExam by id = {id}", id);

            var exam = exams.FirstOrDefault(exams => exams.Id == id);
            if (exam == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(exam);
        }

        [FunctionName("UpdateMedicalExamById")]
        public static async Task<IActionResult> UpdateMedicalExamById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "medicalexam/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("Updating MedicalExam by id = {id}", id);

            var exam = exams.FirstOrDefault(exams => exams.Id == id);
            if (exam == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<MedicalExamUpdateModel>(requestBody);

            exam.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.Description))
            {
                exam.Description = updated.Description;
            }

            return new OkObjectResult(exam);
        }

        [FunctionName("DeleteMedicalExamById")]
        public static async Task<IActionResult> DeleteMedicalExamById(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "medicalexam/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("Getting MedicalExam by id = {id}", id);

            var exam = exams.FirstOrDefault(exams => exams.Id == id);
            
            if (exam == null)
            {
                return new NotFoundResult();
            }

            exams.Remove(exam);

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
}