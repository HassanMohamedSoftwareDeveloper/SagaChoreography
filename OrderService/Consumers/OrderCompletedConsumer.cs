using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Enums;

namespace OrderService.Consumers;

public sealed class OrderCompletedConsumer(OrderDbContext dbContext, ILogger<OrderCompletedEvent> logger) : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var orderId = context.Message.OrderId;
        logger.LogInformation("Start consume {Event} with order id {OrderId}", nameof(OrderCompletedEvent), orderId);
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null) return;

        order.Status = (int)OrderStatus.Completed;
        order.UpdatedDate = DateTime.Now;

        dbContext.Orders.Update(order);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} completed successfully.", orderId);
    }
}
