using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Services.Orders.Commands;
using OrderProcessing.Application.Services.Orders.Queries;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAll()
        {

            var result = await _mediator.Send(new GetOrdersQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery { OrderId = id });
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateOrderCommand request)
        {
            var result = await _mediator.Send(request);
            _unitOfWork.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = result }, result);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateOrderCommand request)
        {
            if (id != request.OrderId)
                return BadRequest();

            await _mediator.Send(request);
            await _unitOfWork.SaveChangesAsync();
            return Ok();   
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, DeleteOrderCommand request)
        {
            if (id != request.OrderId)
                return BadRequest();

            await _mediator.Send(request);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

    }
}
