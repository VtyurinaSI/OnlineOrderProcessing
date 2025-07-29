using FluentValidation;
using OnlineOrder.Application;
using OnlineOrder.Application.Contracts;
using OnlineOrder.Application.DTOs;
using OnlineOrder.Infrastructure;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/orders", async (CreateOrderRequest req,
                                  IOrderService svc,
                                  CancellationToken ct) =>
{
    var order = await svc.CreateOrderAsync(req, ct);
    return Results.Created($"/api/orders/{order.Id}", order);
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapGet("/api/orders/{id:guid}",
           async (Guid id, IOrderService svc, CancellationToken ct)
                => await svc.GetOrderAsync(id, ct) is { } o ? Results.Ok(o) : Results.NotFound())
   .WithName("GetOrder")
   .WithOpenApi();

app.MapGet("/api/orders/{id:guid}/history",
           async (Guid id, IOrderService svc, CancellationToken ct)
                => Results.Ok(await svc.GetHistoryAsync(id, ct)))
   .WithName("GetOrderHistory")
   .WithOpenApi();

app.MapPost("/api/orders/{id:guid}/{action:regex(pay|deliver|cancel|fail)}",
           async (Guid id, string action, IOrderService svc, CancellationToken ct) =>
           {
               var ok = action switch
               {
                   "pay" => await svc.PayAsync(id, ct),
                   "deliver" => await svc.DeliverAsync(id, ct),
                   "cancel" => await svc.CancelAsync(id, ct),
                   _ => await svc.FailAsync(id, ct)
               };
               return ok ? Results.NoContent() : Results.BadRequest();
           })
   .WithName("TriggerOrder")
   .WithOpenApi();

app.Run();
