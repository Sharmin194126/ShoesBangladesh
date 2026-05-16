using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using ShoesBangladesh.Web.Models;
using System.Diagnostics;
using System.Net.Http.Json;

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

        try
        {
            var r = await client.GetAsync("api/LandingPage");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                {
                    var landingData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(j, jsonOptions);
                    if (landingData.TryGetProperty("settings", out var s))
                    {
                        ViewBag.Settings = s;
                    }
                }
            }
        }
        catch (Exception ex) { _logger.LogWarning("LandingPage fetch failed: {0}", ex.Message); }

        // 1. Fetch HomePage Settings (marquee, hero text)
        System.Text.Json.JsonElement? hpSettings = null;
        try
        {
            var r = await client.GetAsync("api/HomePageSettings");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    hpSettings = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("HomePageSettings fetch failed: {0}", ex.Message); }

        // 2. Fetch Banners
        List<System.Text.Json.JsonElement>? banners = null;
        try
        {
            var r = await client.GetAsync("api/Banners");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    banners = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("Banners fetch failed: {0}", ex.Message); }

        // 3. Fetch Categories
        List<System.Text.Json.JsonElement>? categories = null;
        try
        {
            var r = await client.GetAsync("api/Categories");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    categories = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("Categories fetch failed: {0}", ex.Message); }

        // 4. Fetch All Products
        List<System.Text.Json.JsonElement>? allProducts = null;
        try
        {
            var r = await client.GetAsync("api/Products");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    allProducts = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("Products fetch failed: {0}", ex.Message); }

        // 5. Active Display Sections
        List<System.Text.Json.JsonElement>? sections = null;
        try
        {
            var r = await client.GetAsync("api/Dashboard/DisplaySections");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                {
                    var all = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
                    sections = all?.Where(s => s.TryGetProperty("isActive", out var a) && a.GetBoolean()).ToList();
                }
            }
        }
        catch (Exception ex) { _logger.LogWarning("DisplaySections fetch failed: {0}", ex.Message); }

        ViewBag.HpSettings = hpSettings;
        ViewBag.Banners = banners;
        ViewBag.Categories = categories;
        ViewBag.AllProducts = allProducts;
        ViewBag.Sections = sections;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient("ShoesAPI");
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Try to get current user ID if authenticated
        string? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        try
        {
            var url = $"api/Products/{id}/Details";
            if (!string.IsNullOrEmpty(currentUserId)) url += $"?currentUserId={currentUserId}";

            var r = await client.GetAsync(url);
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                var viewModel = System.Text.Json.JsonSerializer.Deserialize<ProductDetailsViewModel>(j, jsonOptions);
                return View(viewModel);
            }
        }
        catch (Exception ex) 
        { 
            _logger.LogError("Product Details fetch failed: {0}", ex.Message); 
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> SendLiveChatMessage(string message)
    {
        var client = _httpClientFactory.CreateClient("ShoesAPI");
        try
        {
            // Forward to API or handle locally
            var contactMsg = new {
                Name = User.Identity?.Name ?? "Guest",
                Email = "chat@shoesbangladesh.com",
                Subject = "Live Chat Message",
                Message = message,
                MessageType = "LiveChat"
            };
            await client.PostAsJsonAsync("api/Dashboard/ContactMessages", contactMsg);
        }
        catch { }
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
