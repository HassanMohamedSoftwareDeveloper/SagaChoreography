using Contracts;
using InventoryService.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Consumers;

public sealed class OrderCreatedConsumer(InventoryDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderId = context.Message.OrderId;
        var customer = context.Message.Customer;
        var total = context.Message.OrderTotal;
        var items = context.Message.Items;

        logger.LogInformation("Start consume {Event} with order id {OrderId}", nameof(OrderCreatedEvent), orderId);
        var itemIds = items
            .Select(x => x.ItemId)
            .ToList();

        var itemStocks = await dbContext.Inventories
             .Where(x => itemIds.Contains(x.ItemId))
             .ToListAsync();

        if (itemStocks is not { } or { Count: 0 })
        {
            await publishEndpoint.Publish(new InventoryFailedEvent(orderId));
            return;
        }

        foreach (var item in items)
        {
            var itemStock = itemStocks.Find(x => x.ItemId == item.ItemId);

            if (itemStock is not { } or { Quantity: <= 0 } || itemStock.Quantity < item.Quantity)
            {
                await publishEndpoint.Publish(new InventoryFailedEvent(orderId));
                return;
            }

            itemStock.Quantity -= item.Quantity;
            itemStock.UpdateDate = DateTime.Now;
        }

        dbContext.Inventories.UpdateRange(itemStocks);

        if (await dbContext.SaveChangesAsync() == 0)
        {
            await publishEndpoint.Publish(new InventoryFailedEvent(orderId));
            return;
        }

        await publishEndpoint.Publish(new InventoryReservedEvent(orderId, customer, total, items));

        logger.LogInformation("Update inventory stock success for order id {OrderId}", orderId);
    }
}
