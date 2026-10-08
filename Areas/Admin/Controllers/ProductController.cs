using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ex03.Data;
using ex03.Models;

namespace ex03.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. DANH SÁCH SẢN PHẨM
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // 2. CHI TIẾT SẢN PHẨM
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

        // 3. THÊM SẢN PHẨM - GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name"
            );

            return View();
        }

        // 4. THÊM SẢN PHẨM - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Product product,
    IFormFile? imageFile)
        {
            // Kiểm tra dữ liệu
            if (!ModelState.IsValid)
            {
                // Load lại danh mục
                ViewBag.CategoryId = new SelectList(
                    await _context.Categories.ToListAsync(),
                    "Id",
                    "Name",
                    product.CategoryId
                );

                return View(product);
            }

            // ==========================
            // UPLOAD ẢNH
            // ==========================
            if (imageFile != null && imageFile.Length > 0)
            {
                string folder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "products"
                );

                // Tự động tạo thư mục nếu chưa có
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                // Tạo tên file mới
                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                string filePath =
                    Path.Combine(folder, fileName);

                // Lưu ảnh vào wwwroot
                using (var stream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn ảnh vào database
                product.Image =
                    "/images/products/" + fileName;
            }

            // ==========================
            // LƯU PRODUCT VÀO DATABASE
            // ==========================

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // ==========================
            // QUAY VỀ DANH SÁCH
            // ==========================

            TempData["Success"] =
                "Thêm sản phẩm thành công!";

            return RedirectToAction(nameof(Index));
        }

        // 5. CHỈNH SỬA - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // 6. CHỈNH SỬA - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product product,
            IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldProduct = await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (oldProduct == null)
                    {
                        return NotFound();
                    }

                    // Nếu có upload ảnh mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(
                            _environment.WebRootPath,
                            "images",
                            "products"
                        );

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string fileName = Guid.NewGuid().ToString()
                                          + Path.GetExtension(imageFile.FileName);

                        string filePath = Path.Combine(
                            uploadsFolder,
                            fileName
                        );

                        using (var stream = new FileStream(
                            filePath,
                            FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        product.Image = "/images/products/" + fileName;

                        // Xóa ảnh cũ
                        if (!string.IsNullOrEmpty(oldProduct.Image))
                        {
                            string oldImagePath = Path.Combine(
                                _environment.WebRootPath,
                                oldProduct.Image.TrimStart('/')
                                    .Replace('/', Path.DirectorySeparatorChar)
                            );

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                    }
                    else
                    {
                        // Không upload ảnh mới -> giữ ảnh cũ
                        product.Image = oldProduct.Image;
                    }

                    _context.Products.Update(product);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool exists = await _context.Products
                        .AnyAsync(p => p.Id == product.Id);

                    if (!exists)
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            ViewBag.CategoryId = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // 7. XÓA - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
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

        // 8. XÓA - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(product.Image))
            {
                string imagePath = Path.Combine(
                    _environment.WebRootPath,
                    product.Image.TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar)
                );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}