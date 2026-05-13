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
                var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(content, options);
                return View(products);
            }

            return View(new List<ProductDTO>());
        }

        public IActionResult Create()
        {
            return View(new ProductDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product, IFormFile? imageFile)
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
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductDTO product, IFormFile? imageFile)
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            await client.DeleteAsync($"api/products/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
