using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateOnly Date {  get; set; }
    public decimal Total { get; set; }
    public OrderState State { get; set; }
    public List<OrderItemDto> Items { get; set; }

}
