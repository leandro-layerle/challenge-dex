namespace ProductCatalog.Domain.Entities;

public class ProductCategory
{
    public int ProductID { get; set; }
    public Product Product { get; set; } = null!;

    public int CategoryID { get; set; }
    public Category Category { get; set; } = null!;
}