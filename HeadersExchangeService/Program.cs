using MassTransit;
using SharedMessages.Messages;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        // 5. Configurazione del bus
        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-headers-exchange"));
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = "headers");
    });
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/orders", async (OrderRequest order, IBus bus) =>
{

    var orderPlacedMessage = new OrderPlaced(order.orderId, order.quantity);
    // headers
    var headers = new Dictionary<string, object>();


    if (order.quantity > 10)
    {
        headers["department"] = "shipping";
        headers["priority"] = "high";
    }
    else
    {
        headers["department"] = "tracking";
        headers["priority"] = "low";
    }

    await bus.Publish(orderPlacedMessage, context =>
    {
        context.Headers.Set("department", headers["department"]);
        context.Headers.Set("priority", headers["priority"]);
    });


    // 4. Restituzione della risposta (201)
    return Results.Created($"/orders/{order.orderId}", orderPlacedMessage);
});







if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public record OrderRequest(Guid orderId, int quantity);
