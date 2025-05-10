using Microsoft.AspNetCore.Mvc;

namespace SalesPro.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserSetup()
        {
            return View();
        } 
        public IActionResult RoleSetup()
        {
            return View();
        }
    }
}
