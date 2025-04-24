using MassTransit;
using SharedMessages.Messages;
using TopicOutQueue1.Consumers;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("q.Queue1", e =>
        {
            e.Consumer<OrderPlacedConsumer>(context);
            e.Bind("order-topic-exchange", x =>
            {
                x.RoutingKey = "order.*"; // * sta per una SOLA parola dopo il punto.
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
