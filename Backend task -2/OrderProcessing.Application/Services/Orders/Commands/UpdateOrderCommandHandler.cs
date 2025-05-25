using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Application.Services.Orders.Commands;

public class UpdateOrderCommand : IRequest
{
    public int OrderId { get; set; }
    public DateOnly Date { get; set; }
    public string CustomerName { get; set; }
    public OrderState State { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<OrderItem> _orderItemsReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;

    public UpdateOrderCommandHandler(
        IRepository<Order> orderRepostiory,
        IReadRepository<Order> orderReadRepository,
        IReadRepository<OrderItem> orderItemsReadRepository,
        IReadRepository<Product> productReadRepository)
    {
        _orderRepostiory = orderRepostiory;
        _orderReadRepository = orderReadRepository;
        _orderItemsReadRepository = orderItemsReadRepository;
        _productReadRepository = productReadRepository;
    }

    public Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _orderReadRepository.GetById(request.OrderId, "OrderItems");
        
        var currentItems = order.OrderItems
            .ToDictionary(oi => new { oi.OrderId, oi.OrderItemId });

        var newItems = request.OrderItems
            .ToDictionary(oi => new { oi.OrderId, oi.OrderItemId });

        foreach (var newItem in request.OrderItems)
        {
            var key = new { newItem.OrderId, newItem.OrderItemId };

            if (currentItems.ContainsKey(key))
            {
                var currentItem = _orderItemsReadRepository.GetById(
                    new object[] { newItem.OrderId, newItem.OrderItemId });
                var product = _productReadRepository.GetById(currentItem.ProductId);
                if (currentItem.Price.Amount == product.CurrentPrice.Amount)
                {
                    if (newItem.Quantity == 0)
                    {
                        order.RemoveOrderItem(newItem.OrderItemId);
                    }
                    else
                    {
                        order.UpdateOrderItem(newItem.OrderItemId, newItem.Quantity);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Item {product.Name} with id {newItem.OrderItemId} price {newItem.Price.Amount} doesn't equal product current price {product.CurrentPrice.Amount}");
                }
            }
            else
            {
                var product = _productReadRepository.GetById(newItem.ProductId);
                if (newItem.Price.Amount == product.CurrentPrice.Amount)
                {
                    if (newItem.Quantity > 0)
                    {
                        order.AddOrderItem(newItem.OrderItemId, request.OrderId, newItem.ProductId,
                            newItem.Quantity, newItem.Price, newItem.Comments);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Item {product.Name} with id {newItem.OrderItemId} price {newItem.Price.Amount} doesn't equal product current price {product.CurrentPrice.Amount}");
                }
            }
        }

        foreach (var currentItem in currentItems.Values)
        {
            var key = new { currentItem.OrderId, currentItem.OrderItemId };
            if (!newItems.ContainsKey(key))
            {
                order.RemoveOrderItem(currentItem.OrderItemId);
            }
        }
        
        order.Update(request.CustomerName, request.Date, request.State);
        _orderRepostiory.Update(order);
       
        return Task.CompletedTask;
    }
}
