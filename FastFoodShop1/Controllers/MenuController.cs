using FastFoodShop1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodShop1.Controllers;

public class MenuController : Controller
{
    private readonly ApplicationDbContext _db;
    public MenuController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? categoryId, string? search, string? price)
    {
        var query = _db.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));
        if (!string.IsNullOrEmpty(price))
        {
            var parts = price.Split('-');
            if (parts.Length == 2 && decimal.TryParse(parts[0], out var min) && decimal.TryParse(parts[1], out var max))
                query = query.Where(p => p.Price >= min && p.Price <= max);
        }

        ViewBag.Categories = await _db.Categories.ToListAsync();
        ViewBag.CategoryId = categoryId;
        ViewBag.Search = search;

        return View(await query.ToListAsync());
    }
}