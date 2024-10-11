using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class GetBmiAdviceFunction
{
    [FunctionName("GetBmiAdvice")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Getting BMI advice.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string category = data?.category ?? req.Query["category"];

        if (string.IsNullOrEmpty(category))
        {
            return new BadRequestObjectResult("Please provide a BMI category.");
        }

        string advice = GetAdvice(category.ToLower());
        if (string.IsNullOrEmpty(advice))
        {
            return new NotFoundObjectResult("No advice found for the specified category.");
        }

        return new OkObjectResult(new { category, advice });
    }

    private static string GetAdvice(string category)
    {
        return category switch
        {
            "underweight" => "Consider increasing your calorie intake with healthy foods.",
            "normal weight" => "Maintain a balanced diet and regular exercise.",
            "overweight" => "Aim to achieve a healthy weight through diet and physical activity.",
            "obese" => "Consult a healthcare provider for a personalized weight loss plan.",
            _ => null,
        };
    }
}
