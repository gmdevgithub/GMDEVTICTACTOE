using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TicTacToeWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _supabaseClient;
        private readonly HttpClient _redisClient;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _supabaseClient = httpClientFactory.CreateClient("Supabase");
            _redisClient = httpClientFactory.CreateClient("UpstashRedis");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Test Supabase connection using REST API
            try
            {
                var supabaseRequest = new HttpRequestMessage(HttpMethod.Get, "/rest/v1/users?select=*");
                supabaseRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("SUPABASE_API_KEY"));

                var supabaseResponse = await _supabaseClient.SendAsync(supabaseRequest);
                if (supabaseResponse.IsSuccessStatusCode)
                {
                    var content = await supabaseResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation("Connected to Supabase API successfully. Response: {Content}", content);
                }
                else
                {
                    _logger.LogError("Failed to connect to Supabase API. Status Code: {StatusCode}", supabaseResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while connecting to Supabase API.");
            }

            // Test Redis connection using REST API
            try
            {
                var redisRequest = new HttpRequestMessage(HttpMethod.Get, "/GET?key=TEST");
                redisRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("UPSTASH_REDIS_TOKEN"));

                var redisResponse = await _redisClient.SendAsync(redisRequest);
                if (redisResponse.IsSuccessStatusCode)
                {
                    var value = await redisResponse.Content.ReadAsStringAsync();
                    if (value == "CONNECTED")
                    {
                        _logger.LogInformation("Connected to Redis database successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to connect to Redis database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to connect to Redis database.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while connecting to Redis database.");
            }

            return Page();
        }
    }
}