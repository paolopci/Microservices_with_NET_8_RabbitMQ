using MassTransit;
using TopicPOutQueue2.Consumers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("q.Queue2", e =>
        {
            e.Consumer<OrderPlacedConsumer>(context);
            e.Bind("order-topic-exchange", x =>
            {
                x.RoutingKey = "order.#"; // # sta per una o più parola/e dopo il punto.
                x.ExchangeType = "topic";
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
