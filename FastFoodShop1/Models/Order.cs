using System.ComponentModel.DataAnnotations.Schema;
namespace FastFoodShop1.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Chờ xác nhận";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}