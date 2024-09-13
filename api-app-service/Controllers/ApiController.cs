using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiController> _logger;
    private readonly string _functionAppRetryUrl;
    private readonly string _functionAppFallbackUrl;

    public ApiController(IHttpClientFactory httpClientFactory, ILogger<ApiController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _functionAppRetryUrl = Environment.GetEnvironmentVariable("FUNCTION_APP_RETRY_URL") ?? throw new ArgumentNullException("FUNCTION_APP_RETRY_URL", "Environment variable is not set.");
        _functionAppFallbackUrl = Environment.GetEnvironmentVariable("FUNCTION_APP_FALLBACK_URL") ?? throw new ArgumentNullException("FUNCTION_APP_FALLBACK_URL", "Environment variable is not set.");
    }

    [HttpPost("perform-operation")]
    public async Task<IActionResult> PerformOperation([FromBody] object requestData)
    {
        var client = _httpClientFactory.CreateClient("FunctionClient");

        try
        {
            // Try calling the functionAppRetry
            _logger.LogInformation("Attempting to call functionAppRetry...");
            var response = await client.PostAsJsonAsync($"{_functionAppRetryUrl}/api/retry-operation", requestData);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully called functionAppRetry.");
                return Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.LogWarning("functionAppRetry returned an error. Calling functionAppFallback...");
                return await CallFallback(client, requestData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception encountered: {ex.Message}. Calling functionAppFallback...");
            return await CallFallback(client, requestData);
        }
    }

    private async Task<IActionResult> CallFallback(HttpClient client, object requestData)
    {
        try
        {
            var response = await client.PostAsJsonAsync($"{_functionAppFallbackUrl}/api/fallback-operation", requestData);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully called functionAppFallback.");
                return Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.LogError("functionAppFallback also failed.");
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in fallback: {ex.Message}");
            return StatusCode(500, "An error occurred while handling the request.");
        }
    }
}
