using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Algolia.Search.Clients;
using Algolia.Search.Models.Common;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Nest;
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
            context.Logger.LogLine($"record: {JsonConvert.SerializeObject(dynamoEvent)}");
            var hubRecords =
                dynamoEvent.Records.Where(x=>x.EventName.Value != "REMOVE")  /* filter for different // entities && x.Dynamodb.Keys["Type"].S == "Payment"*/
                    .SelectMany(record =>
                    {


                        var doc = Document.FromAttributeMap(record.Dynamodb.NewImage);
                        var jsonDoc = doc.ToJson();
                        var transaction = JsonConvert.DeserializeObject<Transaction>(jsonDoc);
                        context.Logger.LogLine($"DynamoDB Record of type {record.EventName}:");
                        context.Logger.LogLine(SerializeStreamRecord(record.Dynamodb));
                        context.Logger.LogLine(jsonDoc);

                        return MapToHubRecords(transaction).ToList();
                    }).ToList();

            if(!hubRecords.Any()) return;

            var response = await ElasticSearchSaveObjects(hubRecords);

            var count = response.Items.Count;
            context.Logger.LogLine($"added {count} records");
            context.Logger.LogLine($"Stream processing complete. Added {hubRecords.Count} records");
        }

        private static async Task<BulkResponse> ElasticSearchSaveObjects(IEnumerable<HubRecord> hubRecords)
        {
            var conn = new ConnectionSettings(new Uri("https://search-hub-v-next-ckccgwkxaxe2z7api7hufhqkxy.eu-west-1.es.amazonaws.com")).DefaultIndex("documents_typed_ordered");

            var esClient = new ElasticClient(conn);
            var response = await esClient.IndexManyAsync(hubRecords);

            return response;
        }

        private static async Task<BatchIndexingResponse> AlgoliaSaveObjects(IEnumerable<SearchableChargeRequested> paymentBatch)
        {
            var client = new SearchClient(new SearchConfig("FDPS0J74WQ", "e4e69e7bc80a5cf2f1ba429c1d058553"));

            var index = client.InitIndex("test");

            var response = await index.SaveObjectsAsync(paymentBatch);
            return response;
        }


        private static async Task<BulkResponse> BonsaiSaveObjects(IEnumerable<SearchableChargeRequested> paymentBatch)
        {
            var url = "https://x025bw0rrh:528ygy4s74@vnext-6440380388.eu-west-1.bonsaisearch.net:443";
            var con = new ConnectionSettings(new Uri(url))
                .DefaultIndex("documents_nested");
            var esClient = new ElasticClient(con);
            var response = await esClient.IndexManyAsync(paymentBatch);

            return response;
        }

        private static string SerializeStreamRecord(StreamRecord streamRecord)
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Serialize(writer, streamRecord);
                return writer.ToString();
            }
        }

        private static IEnumerable<HubRecord> MapToHubRecords(Transaction transaction)
        {
            var actionList = new List<HubRecord>();

                var action = new HubRecord
                {
                    Id = GetMD5Hash($"{transaction.PaymentId.ToString()}_{transaction.ActionId.ToString()}"),
                    Amount = transaction.Value,
                    ActionId = transaction.ActionId.ToString(),
                    Type = "Indie",
                    Action = transaction.ActionCodeId == null ? "UNKNOWN" : ((ActionCode) transaction.ActionCodeId).ToString(),
                    Email = transaction.CustomerEmail,
                    TimeStamp = transaction.Timestamp,
                    CardholderName = transaction.CardHolderName,
                    Bin = int.Parse(transaction.CardNumber.Substring(0, 6)),
                    LastFourCardDigits = short.Parse(transaction.CardNumber.Substring(transaction.CardNumber.Length - 4)),
                    StatusCode = (transaction.ActionCodeId == 4) ? "777" : string.Empty,
                    Reference = transaction.AcquirerReferenceNumber,
                    CardType = transaction.CardType,
                    CurrencyIso = transaction.CurrencyCode,
                    PaymentId = transaction.PaymentId.ToString()
                };
                actionList.Add(action);

                var lastAction = action.Clone();
                lastAction.Type = "Last";
                lastAction.Id = $"{transaction.PaymentId}_L";
                actionList.Add(lastAction);

            return actionList;
        }

        private static string GetMD5Hash(string dataToHash)
        {
            var sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(dataToHash);
                byte[] hash = md5.ComputeHash(inputBytes);

                // step 2, convert byte array to hex string
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }
    }
}