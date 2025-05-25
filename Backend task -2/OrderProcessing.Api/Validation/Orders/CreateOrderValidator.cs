using FluentValidation;
using OrderProcessing.Application.Services.Orders.Commands;

namespace OrderProcessing.Api.Validation.Orders;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(o => o.Id).NotEmpty().WithMessage("OrderId Cannot Be Empty");
        RuleFor(o => o.CustomerName)
            .NotEmpty().WithMessage("Customer Name Cannot Be Empty")
            .MaximumLength(100).WithMessage("Customer Name Cannot Exceed 100 Characters");
        RuleFor(o => o.Date).NotEmpty().WithMessage("Order Date Cannot Be Empty")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Order Date Cannot Be In The Future");
        RuleFor(o => o.State).IsInEnum().WithMessage("Order State Must Be Valid");
        RuleForEach(o => o.OrderItems).ChildRules(orderItem =>
        {
            orderItem.RuleFor(oi => oi.ProductId).NotEmpty().WithMessage("Product Id Cannot Be Empty");
            orderItem.RuleFor(oi => oi.Quantity).GreaterThan(0).WithMessage("Quantity Must Be Greater Than Zero");
            orderItem.RuleFor(oi => oi.Price.Amount).GreaterThan(0).WithMessage("Price Must Be Greater Than Zero");
            orderItem.RuleFor(oi => oi.Comments).MaximumLength(500).WithMessage("Comments Cannot Exceed 50 Characters");
        });
        RuleFor(o => o.OrderItems)
            .Must(items => items.Count > 0).WithMessage("At Least One Order Item Must Be Provided")
            .WithName("Order Items");

    }
}
