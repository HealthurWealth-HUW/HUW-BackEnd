public sealed class ProductAdditionalInfoDto
{
    public long ProductId { get; init; }
    public int Quantity { get; init; }

    public DateTime? ManufacturerDate { get; init; }
    public int? BestBeforeDate { get; init; }
    public string CountryOfOrigin { get; init; }
    public string GTIN { get; init; }
    public string ManufacturerDetails { get; init; }
    public string MarketerDetails { get; init; }
}
