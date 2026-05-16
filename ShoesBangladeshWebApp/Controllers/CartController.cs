using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Models;
using System.Text.Json;
using ShoesBangladesh.API.ViewModels;

namespace ShoesBangladesh.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string CartSessionKey = "ShoesCart";

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private CartViewModel GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson == null ? new CartViewModel() : JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
        }

        private void SaveCart(CartViewModel cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Register", "Account");
            var cart = GetCart();
            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int productId, string? size, int qty = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Home/Details/{productId}" });
            }
            
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }
            
            var product = JsonSerializer.Deserialize<ProductViewModel>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null) return RedirectToAction("Index");

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
            
            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Size = size ?? "",
                    Quantity = qty,
                    UnitPrice = product.DiscountPrice ?? product.Price,
                    VatPercentage = product.VatPercentage
                });
            }
            
            SaveCart(cart);
            TempData["SuccessMessage"] = $"Product added to cart successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string size, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
            if (item != null)
            {
                item.Quantity = quantity > 0 ? quantity : 1;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId, string size)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == productId && i.Size == size);
            SaveCart(cart);
            TempData["Message"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            TempData["Message"] = "Cart cleared.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuyNow(int productId, string? size, int qty = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Order/Checkout?productId={productId}&size={size}&qty={qty}" });
            }
            
            // Clear current cart and add this single item
            HttpContext.Session.Remove(CartSessionKey);
            return await AddToCart(productId, size, qty); // AddToCart already redirects to Index (which is fine, or we can redirect to checkout). Wait, it redirects to Index. Let's fix this.
        }
        
        [HttpGet]
        public async Task<IActionResult> BuyNowDirect(int productId, string? size, int qty = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Order/Checkout?productId={productId}&size={size}&qty={qty}" });
            }
            
            HttpContext.Session.Remove(CartSessionKey);
            
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/products/{productId}");
            if (response.IsSuccessStatusCode)
            {
                var product = JsonSerializer.Deserialize<ProductViewModel>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (product != null)
                {
                    var cart = new CartViewModel();
                    cart.Items.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl,
                        Size = size ?? "",
                        Quantity = qty,
                        UnitPrice = product.DiscountPrice ?? product.Price,
                        VatPercentage = product.VatPercentage
                    });
                    SaveCart(cart);
                }
            }
            
            return RedirectToAction("Checkout", "Order");
        }
    }
}
