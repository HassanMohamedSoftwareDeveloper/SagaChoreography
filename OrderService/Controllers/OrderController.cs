using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken = default)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Select(order => new OrderModel(order.Customer,
                                            order.Total,
                                            ((OrderStatus)order.Status).ToString(),
                                            order.CreatedDate,
                                            order.OrderLines.Select(line => new OrderLineModel(line.ItemId, line.Quantity, line.Price)).ToList()))
            .ToListAsync(cancellationToken);

        return Ok(orders);


    }
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrders(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Id == orderId)
            .Select(order => new OrderModel(order.Customer,
                                            order.Total,
                                            ((OrderStatus)order.Status).ToString(),
                                            order.CreatedDate,
                                            order.OrderLines.Select(line => new OrderLineModel(line.ItemId, line.Quantity, line.Price)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null) return NotFound();

        return Ok(order);


    }
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderModelRequest orderRequest, CancellationToken cancellationToken = default)
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
                Price = line.Price,
            }).ToList()
        };

        dbContext.Orders.Add(order);
        var saveResult = await dbContext.SaveChangesAsync(cancellationToken);
        if (saveResult > 0)
        {
            logger.LogInformation("publish order created event....");
            await publishEndpoint
                  .Publish(new OrderCreatedEvent(order.Id,
                                                 order.Customer,
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
