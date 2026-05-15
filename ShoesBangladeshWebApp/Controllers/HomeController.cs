using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.Web.Models;

namespace ShoesBangladesh.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ShoesAPI");
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // 1. Fetch Banners (safe)
        List<System.Text.Json.JsonElement>? banners = null;
        try
        {
            var bannersResponse = await client.GetAsync("api/Banners");
            if (bannersResponse.IsSuccessStatusCode)
            {
                var bannersJson = await bannersResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(bannersJson))
                    banners = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(bannersJson, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("Banners fetch failed: {0}", ex.Message); }

        // 2. Fetch Featured Products (safe)
        List<System.Text.Json.JsonElement>? products = null;
        try
        {
            var productsResponse = await client.GetAsync("api/Products");
            if (productsResponse.IsSuccessStatusCode)
            {
                var productsJson = await productsResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(productsJson))
                {
                    var allProducts = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(productsJson, jsonOptions);
                    products = allProducts?
                        .Where(p => p.TryGetProperty("isFeatured", out var f) && f.GetBoolean())
                        .Take(8).ToList();
                }
            }
        }
        catch (Exception ex) { _logger.LogWarning("Products fetch failed: {0}", ex.Message); }

        // 3. Fetch Settings (safe)
        System.Text.Json.JsonElement? settings = null;
        try
        {
            var settingsResponse = await client.GetAsync("api/LandingPage/Settings");
            if (settingsResponse.IsSuccessStatusCode)
            {
                var settingsJson = await settingsResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(settingsJson))
                    settings = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(settingsJson, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("Settings fetch failed: {0}", ex.Message); }

        ViewBag.Banners = banners;
        ViewBag.Products = products;
        ViewBag.Settings = settings;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
