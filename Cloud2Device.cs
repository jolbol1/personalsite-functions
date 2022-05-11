using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace Jos.Function
{


    public static class Cloud2Device
    {

        static ServiceClient serviceClient;
        private async static Task SendCloudToDeviceMessageAsync(string message)
        {
            var commandMessage = new
            Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync(Environment.GetEnvironmentVariable("TARGET_DEVICE"), commandMessage);
        }



        [FunctionName("Cloud2Device")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string message = req.Query["message"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            message = message ?? data?.message;

            serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            SendCloudToDeviceMessageAsync(message).Wait();

            string responseMessage = $"Sending message to JosEsp";

            return new OkObjectResult(responseMessage);
        }
    }
}
