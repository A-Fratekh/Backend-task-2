using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderProcessing.Application.Services.Orders.Commands;
using OrderProcessing.Domain.Aggregates.OrderAggregate;

namespace OrderProcessing.Api.Validation.Orders;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(o => o.Date).NotEmpty().WithMessage("Order Date Cannot Be Empty")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Order Date Cannot Be In The Future");
        RuleFor(o => o.State).Equal(OrderState.Draft).WithMessage("Order State Must Be Draft To Update");
        RuleForEach(o => o.OrderItems).ChildRules(orderItem =>
        {
            orderItem.RuleFor(oi => oi.ProductId).NotEmpty().WithMessage("Product Id Cannot Be Empty");
            orderItem.RuleFor(oi => oi.Price.Amount).GreaterThan(0).WithMessage("Price Must Be Greater Than Zero");
            orderItem.RuleFor(oi => oi.Comments).MaximumLength(500).WithMessage("Comments Cannot Exceed 50 Characters");
        });
        RuleFor(o => o.OrderItems)
            .Must(items => items.Count > 0).WithMessage("At Least One Order Item Must Be Provided")
            .WithName("Order Items");

    }
}
