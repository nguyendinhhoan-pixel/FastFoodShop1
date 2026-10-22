using System.ComponentModel.DataAnnotations.Schema;
namespace FastFoodShop1.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalePrice { get; set; }
    public DateTime? SaleEnd { get; set; }

    [NotMapped]
    public bool IsOnSale => SalePrice.HasValue && SalePrice < Price
        && (!SaleEnd.HasValue || SaleEnd.Value.Date >= DateTime.Today);

    [NotMapped]
    public int DiscountPercent => IsOnSale ? (int)Math.Round((1 - SalePrice!.Value / Price) * 100) : 0;
}