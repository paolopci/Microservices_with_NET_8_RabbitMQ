using MassTransit;
using SharedMessages.Messages;


namespace SpippedOutQueue1.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            if (context.Message.Quantity <= 0)
            {
                Console.WriteLine($"Rejected order with ID: {context.Message.OrderId}");
                throw new Exception("Invalid quantity, rejecting the message.");
            }

            Console.WriteLine($"Order received for Exchange : {context.Message.OrderId} and quantity: {context.Message.Quantity}");
            return Task.CompletedTask;
        }
    }
}
