using FastFoodShop1.Data;
using FastFoodShop1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FastFoodShop1.Controllers;

[Authorize]
public class ProductAdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    public ProductAdminController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
        => View(await _db.Products.Include(p => p.Category).OrderBy(p => p.Id).ToListAsync());

    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View("Form", new Product());
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Description,Price,CategoryId,SalePrice,SaleEnd")] Product p, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View("Form", p);
        }
        if (image != null && image.Length > 0)
        {
            var url = await SaveImage(image);
            if (url == null)
            {
                ModelState.AddModelError("", "Chỉ nhận ảnh .jpg, .jpeg, .png, .webp, .gif");
                await LoadCategories();
                return View("Form", p);
            }
            p.ImageUrl = url;
        }
        _db.Products.Add(p);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();
        await LoadCategories();
        return View("Form", p);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Id,Name,Description,Price,CategoryId,SalePrice,SaleEnd")] Product p, IFormFile? image)
    {
        var old = await _db.Products.FindAsync(p.Id);
        if (old == null) return NotFound();
        if (!ModelState.IsValid)
        {
            p.ImageUrl = old.ImageUrl;
            await LoadCategories();
            return View("Form", p);
        }
        if (image != null && image.Length > 0)
        {
            var url = await SaveImage(image);
            if (url == null)
            {
                ModelState.AddModelError("", "Chỉ nhận ảnh .jpg, .jpeg, .png, .webp, .gif");
                p.ImageUrl = old.ImageUrl;
                await LoadCategories();
                return View("Form", p);
            }
            old.ImageUrl = url;
        }
        old.Name = p.Name;
        old.Description = p.Description;
        old.Price = p.Price;
        old.CategoryId = p.CategoryId;
        old.SalePrice = p.SalePrice;
        old.SaleEnd = p.SaleEnd;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p != null)
        {
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategories()
        => ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");

    private async Task<string?> SaveImage(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowed.Contains(ext) || file.Length > 5 * 1024 * 1024) return null;

        var folder = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(folder);
        var name = Guid.NewGuid().ToString("N") + ext;
        using var stream = new FileStream(Path.Combine(folder, name), FileMode.Create);
        await file.CopyToAsync(stream);
        return "/images/products/" + name;
    }
}