using Microsoft.AspNetCore.Mvc;
using ShoesBangladeshWebApp.Request;

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
        public IActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            // Logic will be added when API is ready
            return View(request);
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
