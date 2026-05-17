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
            var r = await client.GetAsync("api/Category");
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
            else
            {
                var error = await r.Content.ReadAsStringAsync();
                return Content($"API Error ({r.StatusCode}): {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Product Details fetch failed: {0}", ex.Message);
            return Content($"Exception: {ex.Message}");
        }
    }

    public async Task<IActionResult> CategoryProducts(int? categoryId, int? typeId, string? priceRange, string? size, string? query)
    {
        var client = _httpClientFactory.CreateClient("ShoesAPI");
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Fetch all products
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
        catch (Exception ex) { _logger.LogWarning("CategoryProducts - Products fetch failed: {0}", ex.Message); }

        // Fetch categories
        List<System.Text.Json.JsonElement>? categories = null;
        try
        {
            var r = await client.GetAsync("api/Category");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    categories = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("CategoryProducts - Categories fetch failed: {0}", ex.Message); }

        // Fetch product types
        List<System.Text.Json.JsonElement>? productTypes = null;
        try
        {
            var r = await client.GetAsync("api/ProductTypes");
            if (r.IsSuccessStatusCode)
            {
                var j = await r.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(j))
                    productTypes = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(j, jsonOptions);
            }
        }
        catch (Exception ex) { _logger.LogWarning("CategoryProducts - ProductTypes fetch failed: {0}", ex.Message); }

        // Build ViewModel
        var vm = new CategoryProductsViewModel
        {
            SelectedCategory = categoryId,
            SelectedType = typeId,
            PriceRange = priceRange,
            SelectedSize = size,
            SearchQuery = query
        };

        // Map categories
        if (categories != null)
        {
            foreach (var cat in categories)
            {
                vm.Categories.Add(new CategoryProductsViewModel.CategoryItem
                {
                    Id = cat.TryGetProperty("id", out var cid) ? cid.GetInt32() : 0,
                    Name = cat.TryGetProperty("name", out var cn) ? cn.GetString() ?? "" : ""
                });
            }
        }

        // Map product types
        if (productTypes != null)
        {
            foreach (var pt in productTypes)
            {
                vm.ProductTypes.Add(new CategoryProductsViewModel.ProductTypeItem
                {
                    Id = pt.TryGetProperty("id", out var ptid) ? ptid.GetInt32() : 0,
                    Name = pt.TryGetProperty("name", out var ptn) ? ptn.GetString() ?? "" : ""
                });
            }
        }

        // Map & filter products
        if (allProducts != null)
        {
            foreach (var p in allProducts)
            {
                var pId = p.TryGetProperty("id", out var pid) ? pid.GetInt32() : 0;
                var pName = p.TryGetProperty("name", out var pn) ? pn.GetString() ?? "" : "";
                var pImg = p.TryGetProperty("imageUrl", out var pi) ? pi.GetString() ?? "" : "";
                var pPrice = p.TryGetProperty("price", out var pp) ? pp.GetDecimal() : 0m;
                var pRegPrice = p.TryGetProperty("regularPrice", out var prp) ? prp.GetDecimal() : pPrice;
                var pDiscount = p.TryGetProperty("discountPrice", out var pdp) && pdp.ValueKind != System.Text.Json.JsonValueKind.Null ? (decimal?)pdp.GetDecimal() : null;
                var pStock = p.TryGetProperty("stockQuantity", out var ps) ? ps.GetInt32() : 0;
                var pStatus = p.TryGetProperty("status", out var pst) ? pst.GetString() ?? "Active" : "Active";
                var pCatId = p.TryGetProperty("categoryId", out var pcid) ? pcid.GetInt32() : 0;
                var pTypeId = p.TryGetProperty("productTypeId", out var ptid2) ? ptid2.GetInt32() : 0;
                var pSizes = p.TryGetProperty("availableSizesJson", out var psz) ? psz.GetString() : null;
                double pRating = 0;
                if (p.TryGetProperty("averageRating", out var pr)) pRating = pr.GetDouble();

                // Apply filters
                if (categoryId.HasValue && pCatId != categoryId.Value) continue;
                if (typeId.HasValue && pTypeId != typeId.Value) continue;
                if (!string.IsNullOrEmpty(query) && !pName.Contains(query, StringComparison.OrdinalIgnoreCase)) continue;

                // Price filter
                var effectivePrice = pDiscount.HasValue && pDiscount.Value > 0 ? pDiscount.Value : pPrice;
                if (priceRange == "low" && effectivePrice >= 500) continue;
                if (priceRange == "mid" && (effectivePrice < 500 || effectivePrice > 2000)) continue;
                if (priceRange == "high" && effectivePrice <= 2000) continue;
                if (priceRange == "very-high" && effectivePrice <= 10000) continue;

                // Size filter
                if (!string.IsNullOrEmpty(size) && !string.IsNullOrEmpty(pSizes) && !pSizes.Contains(size)) continue;

                vm.Products.Add(new CategoryProductsViewModel.CategoryProductItem
                {
                    Id = pId,
                    Name = pName,
                    ImageUrl = pImg,
                    Price = effectivePrice,
                    RegularPrice = pRegPrice > 0 ? pRegPrice : pPrice,
                    DiscountPrice = pDiscount,
                    StockQty = pStock,
                    AverageRating = pRating,
                    Status = pStatus,
                    AvailableSizes = pSizes,
                    CategoryId = pCatId,
                    ProductTypeId = pTypeId
                });
            }
        }

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SendLiveChatMessage(string message)
    {
        var client = _httpClientFactory.CreateClient("ShoesAPI");
        try
        {
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
