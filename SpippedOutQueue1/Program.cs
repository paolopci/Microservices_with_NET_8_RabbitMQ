using MassTransit;
using SpippedOutQueue1.Consumers;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("q.skipped-queue-1", e =>
        {

            // se rimuovo e.Consumer... questo fa si che i messaggi nella coda non vengono consumati
             e.Consumer<OrderPlacedConsumer>(context);


            e.Bind("order-skipped-exchange", e =>
            {
                e.RoutingKey = "order.created";
                e.ExchangeType = "direct";
            });

        });
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
