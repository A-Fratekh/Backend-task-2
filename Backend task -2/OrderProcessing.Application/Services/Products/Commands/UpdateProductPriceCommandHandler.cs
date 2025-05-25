

using MediatR;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Application.Services.Products.Commands;
public class UpdateProductPriceCommand : IRequest
{
    public int ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IReadRepository<Product> _productReadRepository;
    public UpdateProductPriceCommandHandler(IRepository<Product> productRepository, IReadRepository<Product> productReadRepository)
    {
        _productRepository = productRepository;
        _productReadRepository = productReadRepository;
    }
    public Task Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = _productReadRepository.GetById(request.ProductId) ??
            throw new NullReferenceException($"Product with id {request.ProductId} not found");
        product.UpdatePrice(new Money(request.NewPrice));
        _productRepository.Update(product);
        return Task.CompletedTask;
    }
}
