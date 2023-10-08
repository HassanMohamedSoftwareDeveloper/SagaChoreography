using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Enums;

namespace OrderService.Consumers;

public sealed class InventoryFailedConsumer(OrderDbContext dbContext, ILogger<InventoryFailedConsumer> logger) : IConsumer<InventoryFailedEvent>
{
    public async Task Consume(ConsumeContext<InventoryFailedEvent> context)
    {
        var orderId = context.Message.OrderId;
        logger.LogInformation("Start consume {Event} with order id {OrderId}", nameof(InventoryFailedEvent), orderId);
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null) return;

        order.Status = (int)OrderStatus.Canceled;
        order.UpdatedDate = DateTime.Now;

        dbContext.Orders.Update(order);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} canceled successfully.", orderId);
    }
}
