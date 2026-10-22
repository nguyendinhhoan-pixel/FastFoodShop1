using FastFoodShop1.Data;
using FastFoodShop1.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodShop1.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    public CartController(ApplicationDbContext db) => _db = db;

    public IActionResult Index() => View(HttpContext.Session.GetCart());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();

        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item == null)
        {
            cart.Add(new CartItem
            {
                ProductId = p.Id,
                Name = p.Name,
         
                Price = Convert.ToDecimal(p.IsOnSale ? p.SalePrice!.Value : p.Price),
                ImageUrl = p.ImageUrl,
                Quantity = Math.Max(1, quantity)
            });
        }
        else item.Quantity += Math.Max(1, quantity);

        HttpContext.Session.SaveCart(cart);

        // ở lại trang đang xem
        var back = Request.Headers.Referer.ToString();
        return string.IsNullOrEmpty(back) ? RedirectToAction("Index", "Home") : Redirect(back);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(int id, int quantity)
    {
        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
        {
            if (quantity <= 0) cart.Remove(item);
            else item.Quantity = quantity;
            HttpContext.Session.SaveCart(cart);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetCart();
        cart.RemoveAll(c => c.ProductId == id);
        HttpContext.Session.SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.SaveCart(new List<CartItem>());
        return RedirectToAction(nameof(Index));
    }
}