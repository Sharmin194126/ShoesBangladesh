using Microsoft.AspNetCore.Mvc;

namespace ShoesBangladesh.Web.Controllers
{
    public class CartController : Controller
    {
        [HttpGet]
        public IActionResult AddToCart(int productId, string? size, int qty = 1)
        {
            // Implement cart logic (session, database, etc.)
            TempData["Message"] = $"Product added to cart successfully!";
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        [HttpGet]
        public IActionResult BuyNow(int productId, string? size, int qty = 1)
        {
            // Implement direct checkout logic
            return RedirectToAction("Checkout", "Order", new { productId = productId, size = size, qty = qty });
        }
    }
}
