using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            var tableName = "UserResources";
            var itemResult = await client.GetItemAsync(new GetItemRequest()
            {
                TableName = tableName,
                Key = new System.Collections.Generic.Dictionary<string, AttributeValue>()
                {
                    { "UserId", new AttributeValue("test2") },
                    { "ResourceId", new AttributeValue("c1") },
                }
            });


            var document = Document.FromAttributeMap(itemResult.Item);

            document["msg"] = "added2";
            document["UpdatedAt"] = DateTime.Now;

            Console.WriteLine(document.ToJsonPretty());

            var itemResponse = await client.PutItemAsync(new PutItemRequest()
            {
                TableName = tableName,
                Item = document.ToAttributeMap(),
                //ConditionExpression = 
            });

        }
    }
}
