using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShoesBangladeshWebApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
