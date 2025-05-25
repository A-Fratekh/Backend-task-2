using Microsoft.OpenApi.Models;
using MediatR;
using OrderProcessing.Infrastructure;
using OrderProcessing.Application.Services.Orders.Queries;
using OrderProcessing.Api.Validation.Orders;
using OrderProcessing.Api.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Processing", Version = "v1" });
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddInfrastructure(builder.Configuration);

    
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetOrderByIdQueryHandler).Assembly);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Processing API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
