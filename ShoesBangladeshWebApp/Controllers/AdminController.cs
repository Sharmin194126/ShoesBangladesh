using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.ViewModels;
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

        public IActionResult Index()
        {
            return View();
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

    }
}

