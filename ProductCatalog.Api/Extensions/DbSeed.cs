using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.Api.Extensions;

public static class DbSeed
{
    public static async Task SeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Electrónica" },
                new Category { Name = "Hogar" },
                new Category { Name = "Libros" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Products.AnyAsync())
        {
            var elec = await db.Categories.FirstAsync(c => c.Name == "Electrónica");
            var hogar = await db.Categories.FirstAsync(c => c.Name == "Hogar");

            var p = new Product
            {
                Name = "Cámara IP",
                Description = "Cámara de seguridad WiFi",
                Image = "https://picsum.photos/seed/camara/400/300",
                ProductCategories =
                {
                    new ProductCategory { CategoryID = elec.CategoryID },
                    new ProductCategory { CategoryID = hogar.CategoryID }
                }
            };
            db.Products.Add(p);
            await db.SaveChangesAsync();
        }
    }
}
