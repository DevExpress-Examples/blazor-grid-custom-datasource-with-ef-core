using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CustomDataSource.Models;

public partial class NorthwindContext : DbContext {
    public NorthwindContext() { }

    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options) { }

    public virtual DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Order>(entity => {
            entity.HasNoKey();

            entity.Property(e => e.CustomerId)
                .UseCollation("NOCASE")
                .HasColumnType("char(5)")
                .HasColumnName("CustomerID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Freight).HasColumnType("numeric");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.RequiredDate).HasColumnType("datetime");
            entity.Property(e => e.ShipAddress)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(60)");
            entity.Property(e => e.ShipCity)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(15)");
            entity.Property(e => e.ShipCountry)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(15)");
            entity.Property(e => e.ShipName)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(40)");
            entity.Property(e => e.ShipPostalCode)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(10)");
            entity.Property(e => e.ShipRegion)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(15)");
            entity.Property(e => e.ShippedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}