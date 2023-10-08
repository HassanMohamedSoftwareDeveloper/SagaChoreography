// Ignore Spelling: app

using Microsoft.EntityFrameworkCore;

namespace OrderService.Database;

public static class DatabasePreparation
{
    #region Methods :
    public static async Task PopulateDatabasePreparation(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<OrderDbContext>()!;
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
            await context.Database.MigrateAsync();
            Console.WriteLine("--> migrate done successfully...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldn't run migrations: {ex.Message}");
        }
    }
    #endregion

}

