using FastFoodShop1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodShop1.Controllers;

[Authorize(Policy = "SaleAdmin")]
public class SaleController : Controller
{
    private readonly ApplicationDbContext _db;
    public SaleController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Products.Include(p => p.Category).OrderBy(p => p.Id).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, int? discountPercent, DateTime? saleEnd)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();

        if (discountPercent is > 0 and < 100)
        {
            p.SalePrice = Math.Round(p.Price * (100 - discountPercent.Value) / 100m, 0);
            p.SaleEnd = saleEnd;
        }
        else
        {
            p.SalePrice = null;   // bỏ sale
            p.SaleEnd = null;
        }
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Đã lưu: " + p.Name;
        return RedirectToAction(nameof(Index));
    }
}