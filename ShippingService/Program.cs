using MassTransit;
using ShippingService.Consumers;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        // rea (o si collega a) una coda nominata order-placed sul broker.
        cfg.ReceiveEndpoint("order-placed", e =>
        {
            // Qui viene registrato il consumer che si occupa di gestire i messaggi di tipo OrderPlaced.
            e.Consumer<OrderPlacedConsumer>();
        });
    });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

