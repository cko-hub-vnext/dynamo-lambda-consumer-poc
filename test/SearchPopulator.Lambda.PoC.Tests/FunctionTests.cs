using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.TestUtilities;
using Xunit;

namespace SearchPopulator.Lambda.PoC.Tests
{
    public class FunctionTests
    {
        [Fact]
        public async Task TestFunction_WithDynamoStreamRecord()
        {
            var @event = new DynamoDBEvent
            {
                Records = new List<DynamoDBEvent.DynamodbStreamRecord>
                {
                    new DynamoDBEvent.DynamodbStreamRecord
                    {
                        AwsRegion = "eu-central-1",
                        Dynamodb = new StreamRecord
                        {
                            ApproximateCreationDateTime = DateTime.Now,
                            Keys = new Dictionary<string, AttributeValue>
                            {
                                {"pk", new AttributeValue {S = "MyId"}},
                                {"sk", new AttributeValue {S = "MySortKey"}}
                            },
                            NewImage = new Dictionary<string, AttributeValue>
                            {
                                {"pk", new AttributeValue {S = "NewValue"}},
                                {"sk", new AttributeValue {S = "AnotherNewValue"}}
                            },
                            OldImage = new Dictionary<string, AttributeValue>
                            {
                                {"pk", new AttributeValue {S = "OldValue"}},
                                {"sk", new AttributeValue {S = "AnotherOldValue"}}
                            },
                            StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
                        }
                    }
                }
            };

            var context = new TestLambdaContext();
            var function = new Function();

            await function.FunctionHandler(@event, context);

            var testLogger = (TestLambdaLogger) context.Logger;
            Assert.Contains("Stream processing complete", testLogger.Buffer.ToString());
        }

        [Fact]
        public async Task TestFunction_WithEmptyRecordsEvent()
        {
            var @event = new DynamoDBEvent();

            var context = new TestLambdaContext();
            var function = new Function();

            await function.FunctionHandler(@event, context);

            var testLogger = (TestLambdaLogger) context.Logger;
            Assert.Contains("No dynamo stream record to process", testLogger.Buffer.ToString());
        }

        [Fact]
        public async Task TestFunction_WithNullEvent()
        {
            var context = new TestLambdaContext();
            var function = new Function();

            await function.FunctionHandler(null, context);

            var testLogger = (TestLambdaLogger) context.Logger;
            Assert.Contains("No dynamo stream record to process", testLogger.Buffer.ToString());
        }
    }
}