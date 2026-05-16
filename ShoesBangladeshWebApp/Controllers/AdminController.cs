using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using ShoesBangladesh.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoesBangladeshWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/Stats");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<AdminStatsViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(stats);
            }
            return View(new AdminStatsViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetYearlyGoalStats(int year)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/Dashboard/GetYearlyGoalStats?year={year}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetYearlySales(int year)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/Dashboard/GetYearlySales?year={year}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            return BadRequest();
        }

        public async Task<IActionResult> ManageLandingPage()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/LandingPage");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<LandingPageResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(data?.Settings);
            }
            
            var error = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"API Request Failed: {error}";
            return View("Error", new ErrorViewModel { RequestId = response.StatusCode.ToString() });
        }

        public async Task<IActionResult> NewsletterSubscribers()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Newsletter/Subscribers");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var subscribers = JsonSerializer.Deserialize<List<JsonElement>>(json);
                return View(subscribers);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageHomePageSettings()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/HomePageSettings");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var settings = JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(settings);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageHomePageSettings(string marqueeText, string heroHeadline, string heroSubtitle, string heroBtnText, string heroBtnLink, string qualityTitle, string qualityDescription)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var payload = new { 
                MarqueeText = marqueeText, 
                HeroHeadline = heroHeadline, 
                HeroSubtitle = heroSubtitle, 
                HeroBtnText = heroBtnText, 
                HeroBtnLink = heroBtnLink, 
                QualityTitle = qualityTitle,
                QualityDescription = qualityDescription,
                IsActive = true 
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await client.PostAsync("api/HomePageSettings", content);
            TempData["SuccessMessage"] = "Homepage settings updated successfully!";
            return RedirectToAction("ManageHomePageSettings");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SystemSettingsDTO settings, IFormFile? heroImage, IFormFile? bgImage, IFormFile? bannerImage, IFormFile? landingPageLogoImage)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");

            // 1. Handle Hero Image Upload
            if (heroImage != null && heroImage.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = heroImage.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(heroImage.ContentType);
                form.Add(streamContent, "file", heroImage.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    settings.HeroImageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? settings.HeroImageUrl;
                }
            }

            // 2. Handle Background Image Upload
            if (bgImage != null && bgImage.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = bgImage.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(bgImage.ContentType);
                form.Add(streamContent, "file", bgImage.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    settings.HeroBgImageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? settings.HeroBgImageUrl;
                }
            }

            // 3. Handle Banner Image Upload
            if (bannerImage != null && bannerImage.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = bannerImage.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(bannerImage.ContentType);
                form.Add(streamContent, "file", bannerImage.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    settings.BannerImageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? settings.BannerImageUrl;
                }
            }

            // 4. Handle Landing Page Logo Image Upload
            if (landingPageLogoImage != null && landingPageLogoImage.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = landingPageLogoImage.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(landingPageLogoImage.ContentType);
                form.Add(streamContent, "file", landingPageLogoImage.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    settings.LandingPageLogoUrl = uploadJson.GetProperty("imageUrl").GetString() ?? settings.LandingPageLogoUrl;
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(settings), Encoding.UTF8, "application/json");
            await client.PostAsync("api/LandingPage/UpdateSettings", content);
            
            TempData["SuccessMessage"] = "Landing Page settings updated successfully!";
            return RedirectToAction("ManageLandingPage");
        }

        public async Task<IActionResult> OnlinePaymentHistory(string search, string method, string status, string sortBy)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/PaymentHistory");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var history = JsonSerializer.Deserialize<List<PaymentHistoryViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PaymentHistoryViewModel>();

                // Filtering
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    history = history.Where(p => 
                        p.TransactionId.ToLower().Contains(search) || 
                        (p.CustomerName?.ToLower().Contains(search) ?? false) || 
                        (p.CustomerAccount?.ToLower().Contains(search) ?? false)).ToList();
                }

                if (!string.IsNullOrEmpty(method))
                {
                    history = history.Where(p => p.GatewayName == method).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    history = history.Where(p => p.Status == status).ToList();
                }

                // Sorting
                if (sortBy == "amount_desc") history = history.OrderByDescending(p => p.Amount).ToList();
                else if (sortBy == "date_asc") history = history.OrderBy(p => p.PaymentDate).ToList();
                else history = history.OrderByDescending(p => p.PaymentDate).ToList();

                ViewBag.Search = search;
                ViewBag.Method = method;
                ViewBag.Status = status;
                ViewBag.SortBy = sortBy;

                return View(history);
            }
            return View(new List<PaymentHistoryViewModel>());
        }

        public async Task<IActionResult> Messages()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/Messages");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var messages = JsonSerializer.Deserialize<List<ContactMessageViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(messages);
            }
            return View(new List<ContactMessageViewModel>());
        }

        public async Task<IActionResult> ManageReviews()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/Reviews");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var reviews = JsonSerializer.Deserialize<List<ReviewViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(reviews);
            }
            return View(new List<ReviewViewModel>());
        }

        public async Task<IActionResult> FooterSettings()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/Footer");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var footer = JsonSerializer.Deserialize<FooterInfoViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(footer);
            }
            return View(new FooterInfoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> FooterSettings(FooterInfoViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Dashboard/Footer", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Footer settings updated successfully!";
                return RedirectToAction(nameof(FooterSettings));
            }
            return View(model);
        }

        public async Task<IActionResult> ManageDisplaySections()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/DisplaySections");
            
            // Fetch Settings for Logo
            try {
                var sResp = await client.GetAsync("api/LandingPage");
                if (sResp.IsSuccessStatusCode) {
                    var sJson = await sResp.Content.ReadAsStringAsync();
                    var ld = JsonSerializer.Deserialize<JsonElement>(sJson);
                    if (ld.TryGetProperty("settings", out var s)) ViewBag.Settings = s;
                }
            } catch { }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var sections = JsonSerializer.Deserialize<List<DisplaySectionViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(sections);
            }
            return View(new List<DisplaySectionViewModel>());
        }

        public async Task<IActionResult> CreateDisplaySection()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Category");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<JsonElement>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Categories = categories;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDisplaySection(DisplaySectionViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Dashboard/DisplaySections", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Display Section created successfully!";
                return RedirectToAction(nameof(ManageDisplaySections));
            }
            return View(model);
        }

        public async Task<IActionResult> EmployeeSalesOverview()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/EmployeeStats");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<List<EmployeePerformanceViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(stats);
            }
            return View(new List<EmployeePerformanceViewModel>());
        }

        public async Task<IActionResult> EmployeeList()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/Employees");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var employees = JsonSerializer.Deserialize<List<EmployeeViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(employees);
            }
            return View(new List<EmployeeViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(new { OrderId = orderId, Status = status }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Dashboard/UpdateOrderStatus", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Order #{orderId} status updated to {status}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update order status.";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> OrdersByStatus(string status)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            
            string url = status == "All" ? "api/Dashboard/Stats" : $"api/Dashboard/OrdersByStatus/{status}";
            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                List<RecentOrderViewModel> orders;
                
                if (status == "All")
                {
                    var stats = JsonSerializer.Deserialize<AdminStatsViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    orders = stats?.RecentOrders ?? new List<RecentOrderViewModel>();
                }
                else
                {
                    orders = JsonSerializer.Deserialize<List<RecentOrderViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<RecentOrderViewModel>();
                }
                
                ViewBag.Status = status;
                return View("OrdersByStatus", orders);
            }
            return View("OrdersByStatus", new List<RecentOrderViewModel>());
        }

        public Task<IActionResult> PendingOrders() => OrdersByStatus("Pending");
        public Task<IActionResult> ProcessingOrders() => OrdersByStatus("Processing");
        public Task<IActionResult> ShippedOrders() => OrdersByStatus("Shipped");
        public Task<IActionResult> DeliveredOrders() => OrdersByStatus("Delivered");
        public Task<IActionResult> CancelledOrders() => OrdersByStatus("Cancelled");

        [HttpPost]
        public async Task<IActionResult> UpdateSalesGoal(int year, decimal goalAmount)
        {
            // For now, this is a placeholder. You could implement a setting in the API.
            // For demonstration, we'll just return to dashboard.
            TempData["SuccessMessage"] = $"Sales goal for {year} updated to {goalAmount}!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditDisplaySection(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            
            // Fetch Section
            var response = await client.GetAsync($"api/Dashboard/DisplaySections/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(ManageDisplaySections));
            
            var json = await response.Content.ReadAsStringAsync();
            var section = JsonSerializer.Deserialize<DisplaySectionViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Fetch Categories
            var catResponse = await client.GetAsync("api/Category");
            if (catResponse.IsSuccessStatusCode)
            {
                var catJson = await catResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<JsonElement>>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Categories = categories;
            }

            return View(section);
        }

        [HttpPost]
        public async Task<IActionResult> EditDisplaySection(DisplaySectionViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Dashboard/DisplaySections/{model.Id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Display Section updated successfully!";
                return RedirectToAction(nameof(EditDisplaySection), new { id = model.Id });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDisplaySection(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.DeleteAsync($"api/Dashboard/DisplaySections/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Display Section deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete display section.";
            }
            return RedirectToAction(nameof(ManageDisplaySections));
        }
        public async Task<IActionResult> ManageBanners()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Banners");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var banners = JsonSerializer.Deserialize<List<JsonElement>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(banners);
            }
            return View(new List<JsonElement>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateBanner(string? title, string? subtitle, string? linkUrl, int displayOrder, IFormFile imageFile)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            string imageUrl = "/images/hero-bg.png"; // Default

            if (imageFile != null && imageFile.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var stream = imageFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                form.Add(streamContent, "file", imageFile.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    imageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? imageUrl;
                }
            }

            var banner = new { 
                Title = title ?? "New Banner", 
                Subtitle = subtitle ?? "", 
                ImageUrl = imageUrl, 
                LinkUrl = linkUrl ?? "/products", 
                DisplayOrder = displayOrder, 
                IsActive = true 
            };
            
            var content = new StringContent(JsonSerializer.Serialize(banner), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Banners", content);

            if (response.IsSuccessStatusCode) 
            {
                TempData["SuccessMessage"] = "Banner added successfully!";
            }
            else 
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to add banner. API Error: {error}";
            }

            return RedirectToAction(nameof(ManageBanners));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.DeleteAsync($"api/Banners/{id}");
            if (response.IsSuccessStatusCode) TempData["SuccessMessage"] = "Banner deleted successfully!";
            else TempData["ErrorMessage"] = "Failed to delete banner.";
            return RedirectToAction(nameof(ManageBanners));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerAction(int id, string? title, string? linkUrl, IFormFile? imageFile)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            
            // 1. Fetch current banner data from the API
            var getResponse = await client.GetAsync($"api/Banners");
            string imageUrl = "/images/hero-bg.png";
            string subtitle = "";
            int displayOrder = 0;

            if (getResponse.IsSuccessStatusCode)
            {
                var contentStr = await getResponse.Content.ReadAsStringAsync();
                var banners = JsonSerializer.Deserialize<List<JsonElement>>(contentStr);
                
                // Find by ID (checking both casings for robustness)
                var banner = banners?.FirstOrDefault(b => 
                    (b.TryGetProperty("id", out var idProp) && idProp.GetInt32() == id) || 
                    (b.TryGetProperty("Id", out var idProp2) && idProp2.GetInt32() == id)
                );

                if (banner.HasValue)
                {
                    imageUrl = (banner.Value.TryGetProperty("imageUrl", out var img) ? img.GetString() : 
                               banner.Value.TryGetProperty("ImageUrl", out var img2) ? img2.GetString() : "") ?? imageUrl;
                    
                    subtitle = (banner.Value.TryGetProperty("subtitle", out var sub) ? sub.GetString() : 
                               banner.Value.TryGetProperty("Subtitle", out var sub2) ? sub2.GetString() : "") ?? "";
                    
                    displayOrder = (banner.Value.TryGetProperty("displayOrder", out var ord) ? ord.GetInt32() : 
                                   banner.Value.TryGetProperty("DisplayOrder", out var ord2) ? ord2.GetInt32() : 0);
                }
            }

            // 2. Handle Image Upload if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var stream = imageFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                form.Add(streamContent, "file", imageFile.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    imageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? imageUrl;
                }
            }

            // 3. Construct the updated banner object as a Dictionary for maximum compatibility
            var updateData = new Dictionary<string, object>
            {
                { "Id", id },
                { "Title", title ?? "New Banner" },
                { "Subtitle", subtitle },
                { "ImageUrl", imageUrl },
                { "LinkUrl", linkUrl ?? "/products" },
                { "DisplayOrder", displayOrder },
                { "IsActive", true }
            };
            
            var content = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Banners/{id}", content);

            if (response.IsSuccessStatusCode) 
            {
                TempData["SuccessMessage"] = "Banner updated successfully!";
            }
            else 
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to update banner. API Status: {response.StatusCode}. Error: {errorMsg}";
            }

            return RedirectToAction(nameof(ManageBanners));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLogo(IFormFile logoFile)
        {
            if (logoFile != null && logoFile.Length > 0)
            {
                var client = _httpClientFactory.CreateClient("ShoesAPI");
                
                // 1. Upload Logo
                using var form = new MultipartFormDataContent();
                using var stream = logoFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(logoFile.ContentType);
                form.Add(streamContent, "file", logoFile.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    var logoUrl = uploadJson.GetProperty("imageUrl").GetString();

                    // 2. Fetch Current Settings to Update
                    var settingsResponse = await client.GetAsync("api/LandingPage");
                    if (settingsResponse.IsSuccessStatusCode)
                    {
                        var settingsJson = await settingsResponse.Content.ReadAsStringAsync();
                        var landingData = JsonSerializer.Deserialize<JsonElement>(settingsJson);
                        var settings = landingData.GetProperty("settings");

                        var updateDto = new {
                            EidOfferTitle = settings.GetProperty("eidOfferTitle").GetString(),
                            EidOfferSubtitle = settings.GetProperty("eidOfferSubtitle").GetString(),
                            DiscountPercentage = settings.GetProperty("discountPercentage").GetInt32(),
                            EidOfferEndTime = settings.GetProperty("eidOfferEndTime").GetDateTime(),
                            IsOfferActive = settings.GetProperty("isOfferActive").GetBoolean(),
                            CompanyName = settings.GetProperty("companyName").GetString(),
                            CompanyDescription = settings.GetProperty("companyDescription").GetString(),
                            ProductSectionDescription = settings.GetProperty("productSectionDescription").GetString(),
                            FacebookPageLink = settings.GetProperty("facebookPageLink").GetString(),
                            ContactEmail = settings.GetProperty("contactEmail").GetString(),
                            Phone = settings.GetProperty("phone").GetString(),
                            LogoUrl = logoUrl, // New Logo
                            HeroImageUrl = settings.GetProperty("heroImageUrl").GetString(),
                            HeroBgImageUrl = settings.GetProperty("heroBgImageUrl").GetString(),
                            BannerImageUrl = settings.GetProperty("bannerImageUrl").GetString(),
                            BannerTitle = settings.GetProperty("bannerTitle").GetString(),
                            BannerDescription = settings.GetProperty("bannerDescription").GetString(),
                            OfferCardTitle = settings.GetProperty("offerCardTitle").GetString(),
                            OfferCardSubtitle = settings.GetProperty("offerCardSubtitle").GetString(),
                            OfferCardCouponCode = settings.GetProperty("offerCardCouponCode").GetString()
                        };

                        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
                        var finalResponse = await client.PostAsync("api/LandingPage/UpdateSettings", updateContent);
                        
                        if (finalResponse.IsSuccessStatusCode)
                            TempData["SuccessMessage"] = "Website Logo updated successfully!";
                        else
                            TempData["ErrorMessage"] = "Failed to save logo settings.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to upload logo image.";
                }
            }
            return RedirectToAction(nameof(ManageDisplaySections));
        }
    }
}
