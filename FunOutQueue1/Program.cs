using FunOutQueue1.Consumers;
using MassTransit;
using SharedMessages.Messages;

// Coda FunOutQueue #1

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("q.queue-1", e =>
        {
            e.Consumer<OrderPlacedConsumer>(context);

            e.Bind("order-funOut-exchange", x =>
            {
                x.ExchangeType= "fanout";
            });
        });
    });
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.Run();

