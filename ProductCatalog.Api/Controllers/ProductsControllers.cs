using System.Drawing;
using System.Runtime.Intrinsics.X86;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Models.Records;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext appContext) : ControllerBase
{
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        page = page <= 0 ? 1 : page;
        size = size <= 0 ? 10 : Math.Min(size, 100);

        var products = appContext.Products
            .AsNoTracking()
            .Include(p => p.ProductCategories)
            .ThenInclude(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            products = products.Where(p => p.Name.Contains(search));

        var total = await products.CountAsync();

        var pagedProducts = await products.OrderBy(p => p.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new ProductDto
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                Categories = p.ProductCategories
                    .Select(pc => new CategoryDto { CategoryID = pc.CategoryID, Name = pc.Category.Name })
                    .ToList()
            })
            .ToListAsync();

        return Ok(new PagedResult<ProductDto>(pagedProducts, total, page, size));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var p = await appContext.Products.AsNoTracking()
            .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (p is null) return NotFound();

        var dto = new ProductDto
        {
            ProductID = p.ProductID,
            Name = p.Name,
            Description = p.Description,
            Image = p.Image,
            Categories = p.ProductCategories
                .Select(pc => new CategoryDto { CategoryID = pc.CategoryID, Name = pc.Category.Name })
                .ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Image = request.Image
        };

        // Vincular categorías
        if (request.CategoryIds?.Count > 0)
        {
            var validCategoryIds = await appContext.Categories
                .Where(c => request.CategoryIds.Contains(c.CategoryID))
                .Select(c => c.CategoryID)
                .ToListAsync();

            foreach (var cid in validCategoryIds)
                product.ProductCategories.Add(new ProductCategory { CategoryID = cid });
        }

        appContext.Products.Add(product);
        await appContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.ProductID },
            new ProductDto
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Categories = product.ProductCategories.Select(pc =>
                    new CategoryDto { CategoryID = pc.CategoryID, Name = appContext.Categories.Find(pc.CategoryID)!.Name }).ToList()
            });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var product = await appContext.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product is null) return NotFound();

        product.Name = request.Name;
        product.Description = request.Description;
        product.Image = request.Image;

        // Sincronizar categorías
        var newIds = request.CategoryIds ?? [];
        var currentIds = product.ProductCategories.Select(pc => pc.CategoryID).ToHashSet();

        // Eliminar las que ya no están
        product.ProductCategories
            .Where(pc => !newIds.Contains(pc.CategoryID))
            .ToList()
            .ForEach(pc => appContext.ProductCategories.Remove(pc));

        // Agregar nuevas
        var toAdd = newIds.Where(id => !currentIds.Contains(id)).ToList();
        if (toAdd.Count > 0)
        {
            var validIds = await appContext.Categories
                .Where(c => toAdd.Contains(c.CategoryID))
                .Select(c => c.CategoryID)
                .ToListAsync();

            foreach (var cid in validIds)
                product.ProductCategories.Add(new ProductCategory { CategoryID = cid, ProductID = product.ProductID });
        }

        await appContext.SaveChangesAsync();

        // Mapear el DTO (consulta rápida de nombres)
        await appContext.Entry(product).Collection(p => p.ProductCategories).LoadAsync();
        var categorias = await appContext.Categories
            .Where(c => product.ProductCategories.Select(pc => pc.CategoryID).Contains(c.CategoryID))
            .Select(c => new CategoryDto { CategoryID = c.CategoryID, Name = c.Name })
            .ToListAsync();

        return Ok(new ProductDto { ProductID = product.ProductID, Name = product.Name, Description = product.Description, Image = product.Image, Categories = categorias });
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await appContext.Products.FindAsync(id);
        if (product is null) return NotFound();

        appContext.Products.Remove(product);
        await appContext.SaveChangesAsync();
        return NoContent();
    }
}