using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Models;
using System.Text.Json;

namespace ShoesBangladesh.Web.Controllers
{
    public class OrderController : Controller
    {
        private const string CartSessionKey = "ShoesCart";

        public IActionResult Checkout()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            var cart = cartJson == null ? new CartViewModel() : JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();

            return View(cart);
        }
    }
}

