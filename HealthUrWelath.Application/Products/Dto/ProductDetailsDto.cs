public sealed class ProductDetailsDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; }
    public string ProductDescription { get; init; }

    public decimal ProductCost { get; init; }
    public decimal ProductOriginalCost { get; init; }
    public decimal ProductDiscountPercentage { get; init; }
    public int Quantity { get; init; }

    public string ProductImgUrl { get; init; }
    public string ProductImgUrl2 { get; init; }

    public string Brand { get; init; }
    public string ShortDescription { get; init; }
    public bool IsPresciption { get; init; }
    public decimal? GST { get; init; }
    public decimal? ShippingCost { get; init; }

    public string Manufacturer_Date { get; init; }
    public string Manufacturer_Details { get; init; }
    public string Best_Before_Date { get; init; }

}
