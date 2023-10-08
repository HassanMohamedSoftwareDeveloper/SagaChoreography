using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Database;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    #region PROPS :
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    #endregion

    #region Methods :
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ORDERS");

        modelBuilder.Entity<Order>()
            .ToTable("Orders");

        modelBuilder.Entity<Order>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Order>()
           .HasMany(x => x.OrderLines)
           .WithOne(x => x.Order)
           .HasForeignKey(x => x.OrderId);

        modelBuilder.Entity<OrderLine>()
            .ToTable("OrderLines");

        modelBuilder.Entity<OrderLine>()
         .HasKey(x => x.Id);
    }
    #endregion
}
