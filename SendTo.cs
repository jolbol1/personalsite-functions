using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;


namespace Jos.Function
{

    public class Weather
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
        public BsonDateTime time = new BsonDateTime(DateTime.Now);
    }
    public class SendTo
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("SendTo")]
        public void Run([IoTHubTrigger("messages/events", Connection = "IOT_HUB_CONNECT")] EventData message, ILogger log)
        {
            dynamic msg = JObject.Parse(Encoding.UTF8.GetString(message.Body.Array));
            var weather = new Weather
            {
                Id = new ObjectId(),
                temperature = msg.temperature,
                humidity = msg.humidity
            };
            var client = new MongoClient(System.Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
            var database = client.GetDatabase("iot-database");
            var collection = database.GetCollection<Weather>("Weather");
            collection.InsertOne(weather);

            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }
}