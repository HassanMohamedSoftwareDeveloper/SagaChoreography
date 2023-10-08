using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Database;
using OrderService.Entities;
using OrderService.Enums;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(ILogger<OrderController> logger,
                             IPublishEndpoint publishEndpoint,
                             OrderDbContext dbContext) : ControllerBase
{

    #region Actions :
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderModel orderRequest, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("start creating the order....");
        var order = new Order
        {
            Customer = orderRequest.Customer,
            CreatedDate = DateTime.Now,
            Total = orderRequest.OrderLines.Sum(x => x.Quantity * x.Price),
            Status = (int)OrderStatus.New,
            OrderLines = orderRequest.OrderLines.Select(line => new OrderLine
            {
                ItemId = line.ItemId,
                Quantity = line.Quantity,
            }).ToList()
        };

        dbContext.Orders.Add(order);
        var saveResult = await dbContext.SaveChangesAsync(cancellationToken);
        if (saveResult > 0)
        {
            logger.LogInformation("publish order created event....");
            await publishEndpoint
                  .Publish(new OrderCreatedEvent(order.Id,
                                                 DateTime.Now,
                                                 order.Total,
                                                 order.OrderLines
                                                 .Select(x => new OrderItem(x.ItemId, x.Quantity))
                                                 .ToList()),
                                                 cancellationToken
                                                 );
        }
        return Ok();
    }
    #endregion

}
