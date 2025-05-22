using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Application.DTOs;

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Comments { get; set; }
}
