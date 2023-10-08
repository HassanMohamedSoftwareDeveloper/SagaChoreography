using Contracts;
using InventoryService.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Consumers;

public sealed class PaymentFailedConsumer(InventoryDbContext dbContext, ILogger<PaymentFailedConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var orderId = context.Message.OrderId;
        var items = context.Message.Items;

        logger.LogInformation("Start consume {Event} with order id {OrderId}", nameof(PaymentFailedEvent), orderId);

        var itemIds = items
            .Select(x => x.ItemId)
            .ToList();

        var itemStocks = await dbContext.Inventories
             .Where(x => itemIds.Contains(x.ItemId))
             .ToListAsync();

        foreach (var item in items)
        {
            var itemStock = itemStocks.Find(x => x.ItemId == item.ItemId)!;

            itemStock.Quantity += item.Quantity;
            itemStock.UpdateDate = DateTime.Now;
        }

        dbContext.Inventories.UpdateRange(itemStocks);

        await dbContext.SaveChangesAsync();

        await publishEndpoint.Publish(new InventoryReleasedEvent(orderId));

        logger.LogInformation("Release inventory stock success for order id {OrderId}", orderId);
    }
}
