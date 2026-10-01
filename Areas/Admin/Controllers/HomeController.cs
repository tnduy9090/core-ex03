using Microsoft.AspNetCore.Mvc;

namespace ex03.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Tables()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }

        public IActionResult Order()
        {
            return View();
        }
    }
}
