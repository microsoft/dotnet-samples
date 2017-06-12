using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Authentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}
