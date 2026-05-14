using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using ShoesBangladesh.Web.Models;
using System.Text;
using System.Text.Json;

namespace ShoesBangladeshWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/products");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var products = JsonSerializer.Deserialize<IEnumerable<ProductViewModel>>(content, options);
                return View(products);
            }

            return View(new List<ProductViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new ProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel product, IFormFile? imageFile)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");

            // Upload image from desktop if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = imageFile.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                form.Add(streamContent, "file", imageFile.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    product.ImageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? "";
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/products", content);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            await LoadDropdowns();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await LoadDropdowns();
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel product, IFormFile? imageFile)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");

            // Upload new image from desktop if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = imageFile.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                form.Add(streamContent, "file", imageFile.FileName);

                var uploadResponse = await client.PostAsync("api/products/upload", form);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                    var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                    product.ImageUrl = uploadJson.GetProperty("imageUrl").GetString() ?? product.ImageUrl;
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/products/{product.Id}", content);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            await LoadDropdowns();
            return View(product);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Map Category Name
                var catRes = await client.GetAsync($"api/Category/{product.CategoryId}");
                if (catRes.IsSuccessStatusCode) {
                    var catJson = await catRes.Content.ReadAsStringAsync();
                    var category = JsonSerializer.Deserialize<ShoesBangladesh.API.ViewModels.CategoryDTO>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    product.CategoryName = category?.Name ?? "Unknown";
                }

                return View(product);
            }
            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Map Category Name
                var catRes = await client.GetAsync($"api/Category/{product.CategoryId}");
                if (catRes.IsSuccessStatusCode) {
                    var catJson = await catRes.Content.ReadAsStringAsync();
                    var category = JsonSerializer.Deserialize<ShoesBangladesh.API.ViewModels.CategoryDTO>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    product.CategoryName = category?.Name ?? "Unknown";
                }

                return View(product);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.DeleteAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to delete product.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Categories
            var catRes = await client.GetAsync("api/Category");
            if (catRes.IsSuccessStatusCode) {
                ViewBag.Categories = JsonSerializer.Deserialize<IEnumerable<ShoesBangladesh.API.ViewModels.CategoryDTO>>(await catRes.Content.ReadAsStringAsync(), opts);
            }

            // Employees
            var empRes = await client.GetAsync("api/employees");
            if (empRes.IsSuccessStatusCode) {
                ViewBag.Employees = JsonSerializer.Deserialize<IEnumerable<ShoesBangladesh.API.ViewModels.EmployeeViewModel>>(await empRes.Content.ReadAsStringAsync(), opts);
            }

            // Display Sections
            var secRes = await client.GetAsync("api/displaysections");
            if (secRes.IsSuccessStatusCode) {
                ViewBag.DisplaySections = JsonSerializer.Deserialize<IEnumerable<ShoesBangladesh.API.ViewModels.DisplaySectionViewModel>>(await secRes.Content.ReadAsStringAsync(), opts);
            }
        }
    }
}
