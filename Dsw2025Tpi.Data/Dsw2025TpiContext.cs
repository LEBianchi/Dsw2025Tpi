using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(eb =>
        {
            eb.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

            eb.Property(p => p.Email)
            .HasMaxLength(60)
            .IsRequired();


            eb.Property(p => p.PhoneNumber)
            .HasMaxLength(17)
            .IsRequired();

            eb.HasData(

   );
        }); 

        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.Property(P => P.Sku)
            .HasMaxLength(50)
            .IsRequired();

            eb.Property(P => P.Name)
            .HasMaxLength(50)
            .IsRequired();

            eb.Property(p => p.Description)
            .HasMaxLength(200);

            eb.Property(P => P.CurrentUnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();

            eb.Property(p => p.StockQuantity)
            .IsRequired();

        });

        modelBuilder.Entity<Order>(eb =>
        {
            eb.ToTable("Orders");

            eb.Property(P => P.ShippingAddress)
            .HasMaxLength(150)
            .IsRequired();

            eb.Property(P => P.BillingAddress)
            .HasMaxLength(150)
            .IsRequired();

            eb.Property(p => p.TotalAmount)
            .HasPrecision(15, 2)
            .IsRequired();

            eb.Property(p => p.Notes)
            .HasMaxLength(300);
        });

        modelBuilder.Entity<OrderItem>(eb =>
        {
            eb.ToTable("OrderItems");

            eb.Property(p => p.Quantity)
                .IsRequired();

            eb.Property(p => p.UnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();

            eb.Ignore(p => p.Subtotal);

        });
    }
}
