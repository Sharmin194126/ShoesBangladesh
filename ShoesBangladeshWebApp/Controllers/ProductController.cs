using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.ViewModels;
using System.Text;
using System.Text.Json;

namespace ShoesBangladeshWebApp.Controllers
{
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/products", content);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            return View(product);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ProductDTO product)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/products/{product.Id}", content);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            return View(product);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            await client.DeleteAsync($"api/products/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}

