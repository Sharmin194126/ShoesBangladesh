using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace ShoesBangladesh.Web.Services
{
    public class SettingsFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SettingsFilter> _logger;
        private static JsonElement? _cachedSettings;
        private static DateTime _cacheExpiration = DateTime.MinValue;
        private static readonly object _lock = new object();

        public SettingsFilter(IHttpClientFactory httpClientFactory, ILogger<SettingsFilter> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is Controller controller)
            {
                JsonElement? settings = null;
                bool useCache = false;

                lock (_lock)
                {
                    if (_cachedSettings != null && DateTime.UtcNow < _cacheExpiration)
                    {
                        settings = _cachedSettings;
                        useCache = true;
                    }
                }

                if (!useCache)
                {
                    try
                    {
                        var client = _httpClientFactory.CreateClient("ShoesAPI");
                        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var r = await client.GetAsync("api/LandingPage");
                        if (r.IsSuccessStatusCode)
                        {
                            var j = await r.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(j))
                            {
                                var landingData = JsonSerializer.Deserialize<JsonElement>(j, jsonOptions);
                                if (landingData.TryGetProperty("settings", out var s))
                                {
                                    settings = s;
                                    lock (_lock)
                                    {
                                        _cachedSettings = s;
                                        _cacheExpiration = DateTime.UtcNow.AddSeconds(15); // cache settings for 15 seconds
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("SettingsFilter failed to fetch settings: {0}", ex.Message);
                    }
                }

                if (settings != null)
                {
                    controller.ViewBag.Settings = settings;
                }
            }

            await next();
        }
    }
}
