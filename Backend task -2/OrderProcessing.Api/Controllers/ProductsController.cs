using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.Services.Products.Commands;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProductPriceCommand request)
    {
        if (id != request.ProductId)
            return BadRequest();

        await _mediator.Send(request);
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }
}
