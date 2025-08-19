namespace ProductCatalog.Api.Models.Records;

public record CreateProductRequest(
    string Name,
    string? Description,
    string? Image,
    List<int> CategoryIds);

public record UpdateProductRequest(
    string Name,
    string? Description,
    string? Image,
    List<int> CategoryIds);

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);