using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Algolia.Search.Clients;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Newtonsoft.Json;
using SearchPopulator.Lambda.PoC.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SearchPopulator.Lambda.PoC
{
    public class Function
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            if (dynamoEvent?.Records == null)
            {
                context.Logger.LogLine("No dynamo stream record to process");
                return;
            }

            context.Logger.LogLine($"Beginning to process {dynamoEvent?.Records.Count} records...");
            var paymentBatch =
                dynamoEvent.Records.Where(x=>x.EventName.Value != "REMOVE")  /* filter for different entities && x.Dynamodb.Keys["Type"].S == "Payment"*/
                    .Select(record =>
                    {


                        var doc = Document.FromAttributeMap(record.Dynamodb.NewImage);
                        var jsonDoc = doc.ToJson();
                        var chargeRequested = JsonConvert.DeserializeObject<AlgoliaChargeRequested>(jsonDoc);
                        context.Logger.LogLine($"DynamoDB Record of type {record.EventName}:");
                        context.Logger.LogLine(SerializeStreamRecord(record.Dynamodb));
                        context.Logger.LogLine(jsonDoc);

                        return chargeRequested;
                    }).ToList();

            var client = new SearchClient(new SearchConfig("FDPS0J74WQ", "e4e69e7bc80a5cf2f1ba429c1d058553"));

            var index = client.InitIndex("test");

            var response = await index.SaveObjectsAsync(paymentBatch);

            var count = response.Responses.Sum(r => r.ObjectIDs.Count());

            context.Logger.LogLine($"Algolia added {count} records");
            context.Logger.LogLine($"Stream processing complete. Added {paymentBatch.Count} records");
        }

        private static string SerializeStreamRecord(StreamRecord streamRecord)
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Serialize(writer, streamRecord);
                return writer.ToString();
            }
        }
    }
}