using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Azure.Functions.Worker.Http;

namespace function_app_fallback
{
    public class FallbackFunction
{
    private readonly ILogger _logger;
    private readonly IDatabase _redisCache;

    public FallbackFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FallbackFunction>();

        // Get environment variables
        string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? throw new ArgumentNullException("REDIS_CONNECTION_STRING");

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _redisCache = redis.GetDatabase();
    }

    [Function("FallbackOperation")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "fallback-operation")] string input, FunctionContext context)
    {
        _logger.LogInformation("FallbackFunction: Received a request to perform a fallback operation.");

        try
        {
            // Retrieve cached data as fallback
            _logger.LogInformation("Retrieving data from cache...");
            string cachedData = await _redisCache.StringGetAsync("cachedData");

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Successfully retrieved data from cache.");
                await context.GetHttpResponseData().WriteStringAsync(cachedData);
            }
            else
            {
                _logger.LogWarning("No cached data available. Returning default fallback response.");
                await context.GetHttpResponseData().WriteStringAsync("Fallback: No cached data available.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fallback operation failed: {ex.Message}");
            await context.GetHttpResponseData().WriteStringAsync("Fallback operation encountered an error.");
        }
    }
}
}
