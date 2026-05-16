using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Models;
using ShoesBangladeshWebApp.Services;
using System.Text.Json;

namespace ShoesBangladesh.Web.Controllers
{
    public class OrderController : Controller
    {
        private const string CartSessionKey = "ShoesCart";
        private const string ReceiptSessionKey = "OrderReceipt";
        private readonly EmailService _emailService;

        public OrderController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Checkout()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            var cart = cartJson == null ? new CartViewModel() : JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutConfirmed(
            string fullName, string phone, string address,
            string city, string zipCode, string paymentMethod)
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            var cart = cartJson == null ? new CartViewModel() : JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();

            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Checkout");
            }

            // Build receipt
            var receipt = new OrderReceiptViewModel
            {
                InvoiceNumber = $"SB-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                OrderDate = DateTime.Now,
                CustomerName = fullName,
                CustomerEmail = User.Identity?.Name ?? "",
                Phone = phone,
                Address = address,
                City = city,
                ZipCode = zipCode,
                PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "Not Selected" : paymentMethod,
                Cart = cart
            };

            // Save receipt to session for the receipt page
            HttpContext.Session.SetString(ReceiptSessionKey, JsonSerializer.Serialize(receipt));

            // Send emails (fire-and-forget if email fails, still show receipt)
            try
            {
                await _emailService.SendOrderReceiptEmailAsync(receipt);
            }
            catch (Exception ex)
            {
                // Log silently — don't break the order flow
                Console.WriteLine($"Email send failed: {ex.Message}");
            }

            // Clear cart after order placed
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("MoneyReceipt");
        }

        public IActionResult MoneyReceipt()
        {
            var receiptJson = HttpContext.Session.GetString(ReceiptSessionKey);
            if (receiptJson == null)
                return RedirectToAction("Index", "Home");

            var receipt = JsonSerializer.Deserialize<OrderReceiptViewModel>(receiptJson);
            return View(receipt);
        }
    }
}
