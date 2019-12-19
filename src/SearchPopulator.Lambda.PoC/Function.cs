using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SearchPopulator.Lambda.PoC
{
    public class Function
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="dynamoEvent"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            if (dynamoEvent?.Records == null)
            {
                context.Logger.LogLine("No dynamo stream record to process");
                return Task.CompletedTask;
            }

            context.Logger.LogLine($"Beginning to process {dynamoEvent?.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                context.Logger.LogLine($"Event ID: {record.EventID}");
                context.Logger.LogLine($"Event Name: {record.EventName}");

                var streamRecordJson = SerializeStreamRecord(record.Dynamodb);
                context.Logger.LogLine("DynamoDB Record:");
                context.Logger.LogLine(streamRecordJson);
            }

            context.Logger.LogLine("Stream processing complete.");

            return Task.CompletedTask;
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
