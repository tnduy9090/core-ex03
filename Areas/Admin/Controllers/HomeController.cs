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
        public IActionResult DanhMuc()
        {
            return View();
        }
        public IActionResult SanPham()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
