using MassTransit;
using SharedMessages.Messages;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        // 5. Configurazione del bus
        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-exchange"));
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = "topic");
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/orders", async (OrderRequest order, IBus bus) =>
{
    
    var orderPlacedMessage = new OrderPlaced(order.orderId, order.quantity);
    
    await bus.Publish(orderPlacedMessage, context =>
    {
        context.SetRoutingKey(order.quantity > 10 ? "order.shipping" : "order.tracking");
    });

    return Results.Created($"/orders/{order.orderId}", orderPlacedMessage);
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();

public record OrderRequest(Guid orderId, int quantity);