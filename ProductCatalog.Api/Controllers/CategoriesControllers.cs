using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Models.Records;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(AppDbContext appDbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await appDbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { CategoryID = c.CategoryID, Name = c.Name })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto categoryDto)
    {
        var category = new Category { Name = categoryDto.Name };
        appDbContext.Categories.Add(category);
        await appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll),
            new { id = category.CategoryID, },
            new CategoryDto { CategoryID = category.CategoryID, Name = category.Name });
    }
}