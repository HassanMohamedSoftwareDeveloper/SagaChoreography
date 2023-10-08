// Ignore Spelling: app

using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Database;

public static class DatabasePreparation
{
    #region Methods :
    public static async Task PopulateDatabasePreparation(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<InventoryDbContext>()!;
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
            await context.Database.MigrateAsync();
            if (!context.Inventories.Any())
            {
                context.Inventories.AddRange(new List<Inventory>
                {
                    new () {Id=Guid.NewGuid(),ItemId=1,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=2,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=3,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=4,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=5,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=6,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=7,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=8,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=9,Quantity=100,CreatedDate=DateTime.Now},
                    new (){Id=Guid.NewGuid(),ItemId=10,Quantity=100,CreatedDate=DateTime.Now},
                });
                await context.SaveChangesAsync();
            }
            Console.WriteLine("--> migrate done successfully...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldn't run migrations: {ex.Message}");
        }
    }
    #endregion

}
