using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Request;
using ShoesBangladeshWebApp.Response;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ShoesBangladeshWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Customer");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(request);

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authResponse != null && authResponse.IsSuccess)
                {
                    await PerformSignIn(authResponse.User);

                    if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);
                    if (authResponse.User.Role == "Admin") return RedirectToAction("Index", "Admin");
                    return RedirectToAction("Index", "Customer");
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(request);

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/register", content);
            var responseData = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode && authResponse != null && authResponse.IsSuccess)
            {
                await PerformSignIn(authResponse.User);
                TempData["SuccessMessage"] = "Successfully Signed Up!";

                if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);
                if (authResponse.User.Role == "Admin") return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Customer");
            }
            
            ModelState.AddModelError("", authResponse?.Message ?? "Registration failed.");
            return View(request);
        }



        private async Task PerformSignIn(UserResponse user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

