using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ex03.Models;

namespace ex03.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Cart()
    {
        return View();
    }
    public IActionResult Product()
    {
        return View();
    }
    public IActionResult Details()
    {
        return View();
    }
    public IActionResult Checkout()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
