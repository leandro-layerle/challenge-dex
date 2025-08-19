namespace ProductCatalog.Domain.Entities;

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}