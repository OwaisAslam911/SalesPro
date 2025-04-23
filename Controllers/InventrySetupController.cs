using Microsoft.AspNetCore.Mvc;

namespace SalesPro.Controllers
{
    public class InventrySetupController : Controller
    {
        public IActionResult CategorySetup()
        {
            return View();
        }
        public IActionResult BrandSetup()
        {
            return View();
        }
        public IActionResult UnitSetup()
        {
            return View();
        }
        public IActionResult ItemSetup()
        {
            return View();
        }
    }
}
