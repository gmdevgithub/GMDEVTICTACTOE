using Microsoft.EntityFrameworkCore;
using TicTacToeWeb.Data;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Identity services and DB configuration
var supabaseApiUrl = Environment.GetEnvironmentVariable("SUPABASE_API_URL") ?? builder.Configuration["Supabase:ApiUrl"];
var supabaseApiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY") ?? builder.Configuration["Supabase:ApiKey"];
var upstashRedisRestUrl = Environment.GetEnvironmentVariable("UPSTASH_REDIS_HOST") ?? builder.Configuration["UpStashRedis:RestUrl"];
var upstashRedisRestToken = Environment.GetEnvironmentVariable("UPSTASH_REDIS_TOKEN") ?? builder.Configuration["UpStashRedis:RestToken"];

// Log the values to verify they are being read correctly
var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();
logger.LogInformation("Supabase API URL: {SupabaseApiUrl}", supabaseApiUrl);
logger.LogInformation("Supabase API Key: {SupabaseApiKey}", supabaseApiKey);
logger.LogInformation("Upstash Redis Rest URL: {UpstashRedisRestUrl}", upstashRedisRestUrl);
logger.LogInformation("Upstash Redis Rest Token: {UpstashRedisRestToken}", upstashRedisRestToken);

if (string.IsNullOrEmpty(supabaseApiUrl) || string.IsNullOrEmpty(supabaseApiKey))
{
    throw new InvalidOperationException("Supabase API configuration is not configured.");
}

if (string.IsNullOrEmpty(upstashRedisRestUrl) || string.IsNullOrEmpty(upstashRedisRestToken))
{
    throw new InvalidOperationException("Redis REST API configuration is not configured.");
}

// Register HttpClient for Supabase API
builder.Services.AddHttpClient("Supabase", client =>
{
    client.BaseAddress = new Uri(supabaseApiUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseApiKey);
    logger.LogInformation("Supabase HttpClient configured with BaseAddress: {BaseAddress} and Authorization: Bearer {ApiKey}", supabaseApiUrl, supabaseApiKey);
});

// Register HttpClient for Upstash Redis REST API
builder.Services.AddHttpClient("UpstashRedis", client =>
{
    client.BaseAddress = new Uri(upstashRedisRestUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", upstashRedisRestToken);
    logger.LogInformation("Upstash Redis HttpClient configured with BaseAddress: {BaseAddress} and Authorization: Bearer {ApiKey}", upstashRedisRestUrl, upstashRedisRestToken);
});

// You can configure the middleware pipeline using the app object created by builder.Build().
// Middleware is software that is assembled into an application pipeline to handle requests and responses
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

//Authorize the user 
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();