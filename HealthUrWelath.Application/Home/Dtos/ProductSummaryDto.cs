namespace HealthUrWelath.Application.Home.Dtos
{
    public sealed class ProductSummaryDto
    {
        public long ProductId { get; init; }
        public string ProductName { get; init; }

        public decimal ProductCost { get; init; }
        public decimal ProductOriginalCost { get; init; }
        public decimal ProductDiscountPercentage { get; init; }
        public decimal ProductDiscountCost { get; init; }

        public string ProductImgUrl { get; init; }
        public string? ProductImgUrl2 { get; init; }
        public string? Brand { get; init; }
    }

}
