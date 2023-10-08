using InventoryService.Database;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class InventoryController(InventoryDbContext context, ILogger<InventoryController> logger) : ControllerBase
{
    #region Actions :
    [HttpGet("item-stock")]
    public async Task<IActionResult> GetStocks(CancellationToken cancellationToken = default)
    {
        var stocks = await context.Inventories
            .AsNoTracking()
            .Select(stock => new InventoryModel(stock.ItemId, stock.Quantity))
            .ToListAsync(cancellationToken);

        return Ok(stocks);
    }
    [HttpPost("item-stock")]
    public async Task<IActionResult> AddInventoryStock(InventoryModel request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("start update inventory..");
        var itemStock = await context.Inventories.FirstOrDefaultAsync(x => x.ItemId == request.ItemId, cancellationToken);

        if (itemStock == null)
        {
            itemStock = new Entities.Inventory { ItemId = request.ItemId, Quantity = request.Quantity, CreatedDate = DateTime.Now };
            context.Inventories.Add(itemStock);
        }
        else
        {
            itemStock.Quantity += request.Quantity;
            itemStock.UpdateDate = DateTime.Now;
            context.Inventories.Update(itemStock);
        }
        if (await context.SaveChangesAsync(cancellationToken) > 0)
        {
            logger.LogInformation("inventory updated successfully!");
        }
        else
        {
            logger.LogInformation("Failed to update inventory!");
        }
        return Ok();
    }
    #endregion
}
