using MassTransit;
using SharedMessages.Messages;


namespace TopicOutQueue1.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Console.WriteLine($"Order received for shipping : {context.Message.OrderId} and quantity: {context.Message.Quantity}");
            return Task.CompletedTask;
        }
    }
}
