using Microsoft.EntityFrameworkCore;
using PaymentService.Entities;

namespace PaymentService.Database;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    #region PROPS :
    public DbSet<Payment> Payments { get; set; }
    #endregion

    #region Methods :
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("PAYMENTS");

        modelBuilder.Entity<Payment>()
            .ToTable("Payments");

        modelBuilder.Entity<Payment>()
            .HasKey(x => x.Id);
    }
    #endregion
}
