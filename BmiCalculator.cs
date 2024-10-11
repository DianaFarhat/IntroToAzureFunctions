using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BMIAzure
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class BmiCalculator
    {
        [FunctionName("BmiCalculator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string heightStr = null;
            string weightStr = null;

            // Check if it's a GET request, handle query parameters
            if (req.Method == HttpMethods.Get)
            {
                // Read from query string for GET request
                heightStr = req.Query["height"];
                weightStr = req.Query["weight"];
            }
            else if (req.Method == HttpMethods.Post)
            {
                // For POST, read from the request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                heightStr = data?.height;
                weightStr = data?.weight;
            }

            // Validate the inputs
            if (string.IsNullOrEmpty(heightStr) || string.IsNullOrEmpty(weightStr))
            {
                return new BadRequestObjectResult("Please provide height and weight.");
            }

            if (!double.TryParse(heightStr, out double height) || !double.TryParse(weightStr, out double weight))
            {
                return new BadRequestObjectResult("Invalid height or weight format.");
            }

            if (height <= 0 || weight <= 0)
            {
                return new BadRequestObjectResult("Height and weight must be positive values.");
            }

            // Calculate BMI
            double bmi = weight / (height * height);

            return new OkObjectResult($"Your BMI is {bmi:F2}");
        }
    }


}
