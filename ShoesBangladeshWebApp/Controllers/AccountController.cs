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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Priority 1: Admin Dashboard
                if (User.IsInRole("Admin"))
                {
                    return Redirect("/Admin");
                }

                // Priority 2: Safe Return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                
                // Priority 3: Customer Dashboard
                return Redirect("/Customer");
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

                if (authResponse != null && authResponse.IsSuccess && authResponse.User != null)
                {
                    // Clear any existing session/cookies first
                    await HttpContext.SignOutAsync("Cookies");
                    
                    await PerformSignIn(authResponse.User);

                    // Robust role and email check
                    string userRole = authResponse.User.Role?.Trim() ?? "";
                    string userEmail = authResponse.User.Email?.Trim().ToLower() ?? "";

                // Priority 1: Admin Dashboard (Email Safety Net + Role Check)
                if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase) || userEmail == "admin@shoes.com" || userEmail == "admin@gmail.com")
                    {
                        return Redirect("/Admin");
                    }

                    // Priority 2: Safe Return URL (for customers)
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    // Priority 3: Default Customer Dashboard
                    return Redirect("/Customer");
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

            if (response.IsSuccessStatusCode && authResponse != null && authResponse.IsSuccess && authResponse.User != null)
            {
                await PerformSignIn(authResponse.User);
                TempData["SuccessMessage"] = "Successfully Signed Up!";

                string userRole = authResponse.User.Role?.Trim() ?? "";
                string userEmail = authResponse.User.Email?.Trim().ToLower() ?? "";

                // Priority 1: Admin Dashboard (Email Safety Net + Role Check)
                if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase) || userEmail == "admin@shoes.com" || userEmail == "admin@gmail.com")
                {
                    return Redirect("/Admin");
                }

                // Priority 2: Safe Return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                
                // Priority 3: Customer Dashboard
                return Redirect("/Customer");
            }
            
            ModelState.AddModelError("", authResponse?.Message ?? "Registration failed.");
            return View(request);
        }

        private async Task PerformSignIn(UserResponse user)
        {
            string userEmail = user.Email?.Trim().ToLower() ?? "";
            string role = user.Role ?? "Customer";

            // Safety net: Force Admin role for these specific emails
            if (userEmail == "admin@shoes.com" || userEmail == "admin@gmail.com")
            {
                role = "Admin";
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, role), 
                new Claim("FullName", user.FullName ?? "")
            };

            // CRITICAL: Explicitly specify ClaimTypes.Role so IsInRole works correctly
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies", ClaimTypes.Name, ClaimTypes.Role);
            var authProperties = new AuthenticationProperties 
            { 
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

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
