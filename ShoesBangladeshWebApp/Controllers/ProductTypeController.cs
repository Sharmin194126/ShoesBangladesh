using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using System.Text;
using System.Text.Json;

namespace ShoesBangladesh.WebApp.Controllers
{
    public class ProductTypeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductTypeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/producttypes");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var types = JsonSerializer.Deserialize<IEnumerable<ProductTypeViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(types);
            }

            return View(new List<ProductTypeViewModel>());
        }

        public IActionResult Create()
        {
            return View(new ProductTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/producttypes", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product Type created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error creating product type.");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/producttypes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<ProductTypeViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypeViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/producttypes/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product Type updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error updating product type.");
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/producttypes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<ProductTypeViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(model);
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/producttypes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<ProductTypeViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(model);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.DeleteAsync($"api/producttypes/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product Type deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = string.IsNullOrEmpty(error) ? "Error deleting product type." : error;
            return RedirectToAction(nameof(Index));
        }
    }
}
