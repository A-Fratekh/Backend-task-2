namespace OrderProcessing.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateOnly Date {  get; set; }
    public decimal Total { get; set; }
    public string State { get; set; }
    public List<OrderItemDto> Items { get; set; }

}
