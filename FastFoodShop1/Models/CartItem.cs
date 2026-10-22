using System.Text.Json;

namespace FastFoodShop1.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}

public static class CartHelper
{
    const string Key = "cart";

    public static List<CartItem> GetCart(this ISession s)
    {
        var json = s.GetString(Key);
        return json == null ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public static void SaveCart(this ISession s, List<CartItem> cart)
        => s.SetString(Key, JsonSerializer.Serialize(cart));
}