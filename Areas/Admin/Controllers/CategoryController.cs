using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ex03.Data;
using ex03.Models;

namespace ex03.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================
        // 1. DANH SÁCH
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(categories);
        }

        // =========================================
        // 2. CHI TIẾT
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // =========================================
        // 3. THÊM - GET
        // =========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =========================================
        // 4. THÊM - POST
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm danh mục thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // =========================================
        // 5. SỬA - GET
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // =========================================
        // 6. SỬA - POST
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Update(category);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Cập nhật danh mục thành công!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool exists = await _context.Categories
                        .AnyAsync(c => c.Id == category.Id);

                    if (!exists)
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View(category);
        }

        // =========================================
        // 7. XÓA - GET
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // =========================================
        // 8. XÓA - POST
        // =========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Không cho xóa nếu đang có sản phẩm
            if (category.Products != null &&
                category.Products.Any())
            {
                TempData["Error"] =
                    "Không thể xóa danh mục vì đang có sản phẩm thuộc danh mục này!";

                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Xóa danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}