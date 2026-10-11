using ex03.Data;
using ex03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

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

    private const string CartSessionKey = "Cart";

    private List<CartItem> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);

        return string.IsNullOrEmpty(json)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json)
                ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString(
            CartSessionKey,
            JsonSerializer.Serialize(cart));
    }

    // Hiển thị giỏ hàng
    public IActionResult Cart()
    {
        return View(GetCart());
    }

    // Thêm sản phẩm vào giỏ hàng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int id, int quantity = 1)
    {
        if (quantity < 1)
        {
            quantity = 1;
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        var item = cart.FirstOrDefault(p => p.ProductId == id);

        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Quantity = quantity
            });
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Cart));
    }

    // Cập nhật số lượng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCart(int id, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(p => p.ProductId == id);

        if (item != null)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                cart.Remove(item);
            }
            else
            {
                if (quantity < 1)
                {
                    quantity = 1;
                }

                if (quantity > product.Quantity)
                {
                    quantity = product.Quantity;
                }

                if (quantity < 1)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            SaveCart(cart);
        }

        return RedirectToAction(nameof(Cart));
    }

    // Xóa một sản phẩm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int id)
    {
        var cart = GetCart();
        cart.RemoveAll(p => p.ProductId == id);
        SaveCart(cart);

        return RedirectToAction(nameof(Cart));
    }

    // Xóa toàn bộ giỏ hàng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        SaveCart(new List<CartItem>());

        return RedirectToAction(nameof(Cart));
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