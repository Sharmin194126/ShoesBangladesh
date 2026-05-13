using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using ShoesBangladesh.Web.Models;

using System.Text.Json;



namespace ShoesBangladeshWebApp.Controllers
{
    public class LandingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LandingController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int? productId)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/LandingPage");

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"API Request Failed: {errorBody}";
                return View("Error", new ErrorViewModel { RequestId = response.StatusCode.ToString() });
            }



            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<LandingPageResponse>(json, options);

            if (data == null) return View("Error");

            data.TargetProductId = productId;

            // Sorting: If productId is provided, move that product to the top
            if (productId.HasValue)
            {
                var targetProduct = data.Products.FirstOrDefault(p => p.Id == productId.Value);
                if (targetProduct != null)
                {
                    data.Products.Remove(targetProduct);
                    data.Products.Insert(0, targetProduct);
                }
            }

            return View(data);
        }

        public IActionResult OrderNow(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to signup if not registered, with return URL to product details
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Landing/ProductDetails/{productId}" });
            }

            return RedirectToAction("ProductDetails", new { id = productId });
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/Products/{id}");

            if (!response.IsSuccessStatusCode) return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(product);
        }

    }
}
