using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Models;
using ShoesBangladeshWebApp.Services;
using System.Text.Json;

namespace ShoesBangladesh.Web.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OrderController : Controller
    {
        private const string CartSessionKey = "ShoesCart";
        private const string ReceiptSessionKey = "OrderReceipt";
        private readonly EmailService _emailService;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(EmailService emailService, IHttpClientFactory httpClientFactory)
        {
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
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

            // Save Order to Database via API
            try
            {
                var createOrderRequest = new
                {
                    CustomerEmail = User.Identity?.Name ?? "",
                    CustomerName = fullName,
                    Phone = phone,
                    ShippingAddress = address,
                    City = city,
                    ZipCode = zipCode,
                    PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "COD" : paymentMethod,
                    TotalAmount = cart.GrandTotal,
                    Items = cart.Items.Select(i => new {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        VatPercentage = i.VatPercentage
                    }).ToList()
                };

                var client = _httpClientFactory.CreateClient("ShoesAPI");
                var response = await client.PostAsJsonAsync("api/Dashboard/CreateOrder", createOrderRequest);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to save order in DB: " + await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database order save failed: {ex.Message}");
            }

            // Deduct Stock via API
            try
            {
                var deductionRequests = cart.Items.Select(i => new {
                    ProductId = i.ProductId,
                    Size = i.Size,
                    Quantity = i.Quantity
                }).ToList();

                var client = _httpClientFactory.CreateClient("ShoesAPI");
                var response = await client.PostAsJsonAsync("api/Products/DeductStock", deductionRequests);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to deduct stock: " + await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stock deduction failed: {ex.Message}");
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
