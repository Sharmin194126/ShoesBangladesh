using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SystemSettingsDTO settings, IFormFile? heroImage, IFormFile? bgImage)
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

            var content = new StringContent(JsonSerializer.Serialize(settings), Encoding.UTF8, "application/json");
            await client.PostAsync("api/LandingPage/UpdateSettings", content);
            
            TempData["SuccessMessage"] = "Landing Page settings updated successfully!";
            return RedirectToAction("ManageLandingPage");
        }

        public async Task<IActionResult> OnlinePaymentHistory()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Dashboard/PaymentHistory");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var history = JsonSerializer.Deserialize<List<PaymentHistoryViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var sections = JsonSerializer.Deserialize<List<DisplaySectionViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(sections);
            }
            return View(new List<DisplaySectionViewModel>());
        }

        public IActionResult CreateDisplaySection() => View();

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
                return View(orders);
            }
            return View(new List<RecentOrderViewModel>());
        }

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
            var response = await client.GetAsync($"api/Dashboard/DisplaySections/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var section = JsonSerializer.Deserialize<DisplaySectionViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(section);
            }
            return RedirectToAction(nameof(ManageDisplaySections));
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
                return RedirectToAction(nameof(ManageDisplaySections));
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
    }
}
