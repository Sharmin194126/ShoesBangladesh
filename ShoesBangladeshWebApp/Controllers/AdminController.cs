using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;


using System.Text.Json;
using System.Text;

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
        public async Task<IActionResult> UpdateSettings(SystemSettingsDTO settings)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
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

    }
}
