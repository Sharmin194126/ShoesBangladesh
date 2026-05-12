using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Request;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ShoesBangladeshWebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            // Simulation of role-based login (This will later be replaced by API call)
            string role = "";
            if (request.Email == "admin@shoes.com" && request.Password == "123")
            {
                role = "Admin";
            }
            else if (request.Email == "user@shoes.com" && request.Password == "123")
            {
                role = "Customer";
            }

            if (!string.IsNullOrEmpty(role))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.Email),
                    new Claim(ClaimTypes.Role, role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                if (role == "Admin") return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Customer");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(request);
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

        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            // Logic will be added when API is ready
            return View(request);
        }
    }
}
