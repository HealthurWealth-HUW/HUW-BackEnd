namespace HealthUrWealth.Infrastructure.Discovery.Read.Models;

internal sealed class DiscoveryProductFlatRow
{
    // Super category
    public long SuperCategoryId { get; init; }
    public string SuperCategoryName { get; init; }

    // Category
    public long CategoryId { get; init; }
    public string CategoryName { get; init; }

    // Product
    public long ProductId { get; init; }
    public string ProductName { get; init; }

    public decimal ProductCost { get; init; }
    public decimal ProductOriginalCost { get; init; }
    public decimal ProductDiscountPercentage { get; init; }
    public decimal ProductDiscountCost { get; init; }

    public string ProductImgUrl { get; init; }
    public string? ProductImgUrl2 { get; init; }
}
