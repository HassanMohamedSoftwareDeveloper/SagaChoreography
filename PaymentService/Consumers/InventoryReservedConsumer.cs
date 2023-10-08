using Contracts;
using MassTransit;
using PaymentService.Database;
using PaymentService.Entities;

namespace PaymentService.Consumers;

public sealed class InventoryReservedConsumer(PaymentDbContext dbContext, ILogger<InventoryReservedConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<InventoryReservedEvent>
{
    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        var orderId = context.Message.OrderId;
        var total = context.Message.Total;
        var user = context.Message.Customer;
        var items = context.Message.Items;
        try
        {
            logger.LogInformation("Start consume {Event} with order id {OrderId}", nameof(InventoryReservedEvent), orderId);
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = total,
                User = user,
                PaymentDate = DateTime.Now,
            };
            dbContext.Payments.Add(payment);
            if (await dbContext.SaveChangesAsync() == 0)
            {
                await publishEndpoint.Publish(new PaymentFailedEvent(orderId, items));
                return;
            }
            await publishEndpoint.Publish(new OrderCompletedEvent(orderId));
            logger.LogInformation("payment done for Order {OrderId} successfully.", orderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payment has exception!");
            await publishEndpoint.Publish(new PaymentFailedEvent(orderId, items));
        }
    }
}
