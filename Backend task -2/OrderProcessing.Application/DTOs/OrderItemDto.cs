
namespace OrderProcessing.Application.DTOs;

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Comments { get; set; }
}
