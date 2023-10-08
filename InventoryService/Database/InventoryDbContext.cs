using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Database;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    #region PROPS :
    public DbSet<Inventory> Inventories { get; set; }
    #endregion

    #region Methods :
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("INV");

        modelBuilder.Entity<Inventory>()
            .ToTable("Inventories");

        modelBuilder.Entity<Inventory>()
            .HasKey(x => x.Id);
    }
    #endregion
}
