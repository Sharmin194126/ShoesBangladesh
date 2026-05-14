using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShoesBangladeshWebApp.Controllers
{
    [Authorize] // Allow all authenticated users to hit this, then we check role
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return Redirect("/Admin");
            }
            
            if (!User.IsInRole("Customer"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }
    }
}
