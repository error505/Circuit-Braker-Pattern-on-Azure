using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using StackExchange.Redis;


namespace function_app_retry
{
    public class RetryFunction
    {
        private readonly ILogger _logger;
        private readonly CloudTableClient _tableClient;
        private readonly IDatabase _redisCache;

        public RetryFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RetryFunction>();

            // Get environment variables
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new ArgumentNullException("AzureWebJobsStorage");
            string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? throw new ArgumentNullException("REDIS_CONNECTION_STRING");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisCache = redis.GetDatabase();
        }

        [Function("RetryOperation")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "retry-operation")] string input, FunctionContext context)
        {
            _logger.LogInformation("RetryFunction: Received a request to perform an operation.");

            try
            {
                // Simulate database operation
                _logger.LogInformation("Performing database operation...");
                await PerformDatabaseOperation(input);

                // Update cache after successful operation
                _logger.LogInformation("Updating cache...");
                await _redisCache.StringSetAsync("cachedData", input);

                _logger.LogInformation("Operation completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Operation failed: {ex.Message}");
                throw; // Rethrow exception to trigger retry/circuit breaker logic
            }
        }

        private async Task PerformDatabaseOperation(string data)
        {
            // Simulate a database write or read operation
            CloudTable table = _tableClient.GetTableReference("dataTable");
            var insertOperation = TableOperation.InsertOrMerge(new DynamicTableEntity("PartitionKey", "RowKey", "*", new Dictionary<string, EntityProperty> { { "Data", new EntityProperty(data) } }));
            await table.ExecuteAsync(insertOperation);
        }
    }
}
