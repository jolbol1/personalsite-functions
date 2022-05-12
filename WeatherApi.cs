using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Jos.Function
{
    public static class WeatherApi
    {
        [FunctionName("WeatherApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var client = new MongoClient(System.Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
            var database = client.GetDatabase("iot-database");
            var collection = database.GetCollection<Weather>("Weather");

            var documents = collection.Find(h => h.temperature != null && h.humidity != null).ToList();


            return new OkObjectResult(documents);
        }
    }
}
