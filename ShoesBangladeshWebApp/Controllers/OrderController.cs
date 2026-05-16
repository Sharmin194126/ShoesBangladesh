using Microsoft.AspNetCore.Mvc;

namespace ShoesBangladesh.Web.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Checkout(int productId, string? size, int qty)
        {
            // Placeholder for checkout page
            ViewBag.ProductId = productId;
            ViewBag.Size = size;
            ViewBag.Qty = qty;
            return View();
        }
    }
}
