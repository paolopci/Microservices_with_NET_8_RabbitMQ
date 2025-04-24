using HeadersOutQueue1.Consumers;
using MassTransit;


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

            e.Bind("order-headers-exchange", x =>
            {
                x.ExchangeType = "headers";
                x.SetBindingArgument("department","shipping");
                x.SetBindingArgument("priority","high");
                x.SetBindingArgument("x-match","all");
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
