using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DxBlazorApplication1.Models;

public partial class NorthwindContext : DbContext {
    public NorthwindContext() { }

    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options) { }

    public virtual DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=C:\\Northwind.db");

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