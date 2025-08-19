using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(p =>
        {
            p.ToTable("Product");
            p.HasKey(k => k.ProductID);
            p.Property(k => k.Name).HasMaxLength(200).IsRequired();
            p.Property(k => k.Image).HasMaxLength(500);
            p.HasIndex(k => k.Name).HasDatabaseName("IX_Product_Name");
        });

        modelBuilder.Entity<Category>(e =>
       {
           e.ToTable("Category");
           e.HasKey(c => c.CategoryID);
           e.Property(c => c.Name).IsRequired().HasMaxLength(100);
           e.HasIndex(c => c.Name).IsUnique();
       });

         modelBuilder.Entity<ProductCategory>(e =>
        {
            e.ToTable("ProductCategory");
            e.HasKey(pc => new { pc.ProductID, pc.CategoryID });

            e.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(pc => pc.CategoryID).HasDatabaseName("IX_ProductCategory_CategoryID");
        });

    }
}