using Microsoft.AspNetCore.Mvc;

namespace SalesPro.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
