using Contracts;
using InventoryService.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Consumers;

public sealed class OrderCreatedConsumer(InventoryDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        logger.LogInformation("Start consume OrderCreatedEvent with order id{orderId}", context.Message.OrderId);
        var itemIds = context.Message.Items
            .Select(x => x.ItemId)
            .ToList();

        var itemStocks = await dbContext.Inventories
             .Where(x => itemIds.Contains(x.ItemId))
             .ToListAsync();

        if (itemStocks is not { } or { Count: 0 })
        {
            logger.LogInformation("No stock and start order cancellation process with order id{orderId}", context.Message.OrderId);
            await publishEndpoint.Publish(new OrderCanceledEvent(context.Message.OrderId));
            return;
        }

        foreach (var item in context.Message.Items)
        {
            var itemStock = itemStocks.Find(x => x.ItemId == item.ItemId);

            if (itemStock is not { } or { Quantity: <= 0 } || itemStock.Quantity < item.Quantity)
            {
                logger.LogInformation("No stock and start order cancellation process with order id{orderId}", context.Message.OrderId);
                await publishEndpoint.Publish(new OrderCanceledEvent(context.Message.OrderId));
                return;
            }

            itemStock.Quantity -= item.Quantity;
            itemStock.UpdateDate = DateTime.Now;

        }
        dbContext.Inventories.UpdateRange(itemStocks);
        if (await dbContext.SaveChangesAsync() == 0)
        {
            logger.LogInformation("Failed to update inventory stock and start order cancellation process with order id{orderId}", context.Message.OrderId);
            await publishEndpoint.Publish(new OrderCanceledEvent(context.Message.OrderId));
        }

        logger.LogInformation("Update inventory stock success for order id {orderId}", context.Message.OrderId);
    }
}
