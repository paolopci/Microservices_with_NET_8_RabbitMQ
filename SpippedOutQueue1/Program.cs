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

            // provo a spedire il messaggio per 3 volte (con intervallo di 5 secondi tra un tentativo e altro)
            // e poi lo metto in una coda di errore
            //e.UseMessageRetry(r=>
            //    r.Exponential(3,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(30),TimeSpan.FromSeconds(5)));



            // vedi https://chatgpt.com/c/680b3858-0040-8001-afed-4d92d48c32ce
            e.UseKillSwitch(options => options
                .SetActivationThreshold(10)
                .SetTripThreshold(0.15)
                .SetRestartTimeout(m: 1));
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
