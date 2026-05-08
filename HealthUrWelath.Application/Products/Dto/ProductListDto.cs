public sealed class ProductListDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; }
    public string Brand { get; init; }

    public decimal ProductCost { get; init; }
    public decimal ProductOriginalCost { get; init; }
    public decimal ProductDiscountPercentage { get; init; }

    public string ProductImgUrl { get; init; }

    public int Quantity { get; init; }
    public bool IsFeaturedProduct { get; init; }
    public bool IsSold { get; init; }

    public int CategoryId { get; init; }
    public string CategoryName { get; init; }

    public int SubCategoryId { get; init; }
    public string SubCategoryName { get; init; }

    public DateTime UpdatedOn { get; init; }
}
