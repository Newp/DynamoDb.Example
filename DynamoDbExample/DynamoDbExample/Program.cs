using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamoDbExample
{
    class Program
    {
        static readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        static async Task Main(string[] args)
        {
            string tableName = "UserResources";

            //var item = await GetItemAsync<JObject>(tableName, "test33", "c8fb094f-ee89-4817-9aec-559e6a4164a9");
            //item["msg"]= "test6";
            //item["UserId"] = "test44";
            //await PutItemAsync(tableName, item, "UpdatedAt");
            await PutItemAsync(tableName, new
            {
                UserId = "test77",
                Message = "한글 잘되나",
                ResourceId = Guid.NewGuid().ToString(),
                ResourceType = "Field",
                UpdatedAt = DateTime.Now,
            }, "UpdatedAt" );
            //for (int i = 0; i < 100000; i++)
            //{

            //}

        }

        static async Task PutItemAsync<T>(string tableName, T item, string conditionName)
        {
            var attributeValues = AttributeMapConverter.Serialize(item);
            var conditionValue = attributeValues[conditionName];
            
            var itemResponse = await client.PutItemAsync(new PutItemRequest()
            {
                TableName = tableName,
                Item = attributeValues,
                ConditionExpression = $"attribute_not_exists(ResourceId) or {conditionName} = :{conditionName}",
                ExpressionAttributeValues = new System.Collections.Generic.Dictionary<string, AttributeValue>()
                {
                    { ':' + conditionName, conditionValue }
                }
            });
        }

        static async Task<T> GetItemAsync<T>(string tableName, string userId, string resourceId)
        {
            var itemResult = await client.GetItemAsync(new GetItemRequest()
            {
                TableName = tableName,
                Key = new System.Collections.Generic.Dictionary<string, AttributeValue>()
                {
                    { "UserId", new AttributeValue(userId) },
                    { "ResourceId", new AttributeValue(resourceId) },
                }
            });

            var result = AttributeMapConverter.Deserialize<T>(itemResult.Item);
            return result;
        }


    }
    public class AttributeMapConverter
    {
        public static Dictionary<string, AttributeValue> Serialize<T>(T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var document = Document.FromJson(json);
            
            var result = document.ToAttributeMap();
            return result;
        }

        public static T Deserialize<T>(Dictionary<string, AttributeValue> attributeMap)
        {
            var document = Document.FromAttributeMap(attributeMap);
            var json = document.ToJson();

            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public enum Test
    {
        A,
        B,
    }
}
