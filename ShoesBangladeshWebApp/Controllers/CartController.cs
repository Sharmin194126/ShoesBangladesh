using Microsoft.AspNetCore.Mvc;

namespace ShoesBangladesh.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Register", "Account");
            return View();
        }

        [HttpGet]
        public IActionResult AddToCart(int productId, string? size, int qty = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Home/Details/{productId}" });
            }
            
            // Implement cart logic here (session, database, etc.)
            TempData["SuccessMessage"] = $"Product added to cart successfully!";
            return RedirectToAction("Index"); // Go to cart page
        }

        [HttpGet]
        public IActionResult BuyNow(int productId, string? size, int qty = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { returnUrl = $"/Order/Checkout?productId={productId}&size={size}&qty={qty}" });
            }
            
            return RedirectToAction("Checkout", "Order", new { productId = productId, size = size, qty = qty });
        }
    }
}
