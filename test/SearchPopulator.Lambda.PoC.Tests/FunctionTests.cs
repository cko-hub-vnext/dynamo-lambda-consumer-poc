using System;
using System.Collections.Generic;
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
        public void TestFunction_WithNullEvent()
        {
            var context = new TestLambdaContext();
            var function = new Function();

            function.FunctionHandler(null, context);

            var testLogger = (TestLambdaLogger)context.Logger;
            Assert.Contains("No dynamo stream record to process", testLogger.Buffer.ToString());
        }

        [Fact]
        public void TestFunction_WithEmptyRecordsEvent()
        {
            var @event = new DynamoDBEvent();

            var context = new TestLambdaContext();
            var function = new Function();

            function.FunctionHandler(@event, context);

            var testLogger = (TestLambdaLogger)context.Logger;
            Assert.Contains("No dynamo stream record to process", testLogger.Buffer.ToString());
        }

        [Fact]
        public void TestFunction_WithDynamoStreamRecord()
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
                            Keys = new Dictionary<string, AttributeValue> {{"id", new AttributeValue {S = "MyId"}}},
                            NewImage = new Dictionary<string, AttributeValue>
                            {
                                {"field1", new AttributeValue {S = "NewValue"}},
                                {"field2", new AttributeValue {S = "AnotherNewValue"}}
                            },
                            OldImage = new Dictionary<string, AttributeValue>
                            {
                                {"field1", new AttributeValue {S = "OldValue"}},
                                {"field2", new AttributeValue {S = "AnotherOldValue"}}
                            },
                            StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
                        }
                    }
                }
            };

            var context = new TestLambdaContext();
            var function = new Function();

            function.FunctionHandler(@event, context);

            var testLogger = (TestLambdaLogger)context.Logger;
            Assert.Contains("Stream processing complete", testLogger.Buffer.ToString());
        }
    }
}
