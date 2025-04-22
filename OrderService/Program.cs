using MassTransit;
using SharedMessages.Messages;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 1. Definizione della route e del verb HTTP
// MapPost("/orders", …): registra un endpoint
// che risponde a richieste HTTP POST sull’URL /orders.
// Il delegate associato viene eseguito in modo asincrono all’arrivo di una richiesta.
app.MapPost("/orders", async (OrderRequest order, IBus bus) =>
{
    // 2. Creazione del messaggio da pubblicare
    var orderPlacedMessage = new OrderPlaced(order.orderId, order.quantity);
    // 3. Pubblicazione del messaggio
    await bus.Publish(orderPlacedMessage);
    // 4. Restituzione della risposta (201)
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