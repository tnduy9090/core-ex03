using ex03.Data;
using ex03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ex03.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    // Inject database context
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(
        int productId,
        int quantity,
        string customerName,
        string phoneNumber,
        string address)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        if (quantity < 1 || quantity > product.Quantity)
        {
            ModelState.AddModelError(
                "",
                "Số lượng đặt hàng không hợp lệ."
            );

            return View(product);
        }

        if (string.IsNullOrWhiteSpace(customerName) ||
            string.IsNullOrWhiteSpace(phoneNumber) ||
            string.IsNullOrWhiteSpace(address))
        {
            ModelState.AddModelError(
                "",
                "Vui lòng nhập đầy đủ thông tin nhận hàng."
            );

            return View(product);
        }

        var order = new Order
        {
            CustomerName = customerName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Address = address.Trim(),
            TotalAmount = product.Price * quantity,
            Status = "Chờ xử lý"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var orderDetail = new OrderDetail
        {
            OrderId = order.Id,
            ProductId = product.Id,
            Quantity = quantity,
            Price = product.Price
        };

        _context.OrderDetails.Add(orderDetail);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {order.Id}";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Product()
    {
        return View();
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
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
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}