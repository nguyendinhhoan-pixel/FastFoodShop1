using System.Diagnostics;
using FastFoodShop1.Data;
using FastFoodShop1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodShop1.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = await _db.Categories.ToListAsync();
        ViewBag.Latest = await _db.Products.Include(p => p.Category)
             .OrderByDescending(p => p.Id).Take(8).ToListAsync();
        ViewBag.Combo = await _db.Products.Include(p => p.Category)
             .Where(p => p.Category!.Name == "Combo")
             .OrderBy(p => p.Price).Take(4).ToListAsync();
        ViewBag.Sale = await _db.Products.Include(p => p.Category)
            .Where(p => p.SalePrice != null && p.SalePrice < p.Price
                     && (p.SaleEnd == null || p.SaleEnd >= DateTime.Now))
            .OrderBy(p => p.Id).Take(8).ToListAsync();
        var featured = await _db.Products.Include(p => p.Category)
            .OrderBy(p => p.Id).Take(8).ToListAsync();
        var saleList = (List<Product>)ViewBag.Sale;
        ViewBag.SaleEnd = saleList
            .Where(p => p.SaleEnd != null)
            .Select(p => p.SaleEnd)
            .Min();
        return View(featured);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}