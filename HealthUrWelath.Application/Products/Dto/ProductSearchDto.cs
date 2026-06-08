namespace HealthUrWelath.Application.Products.Dto
{
    public sealed class ProductSearchDto
    {
        public long ProductId { get; init; }

        public string ProductName { get; init; }

        public string Brand { get; init; }

        public decimal ProductCost { get; init; }
        public int Quantity { get; init; }

        public string ProductImgUrl { get; init; }

        public decimal ProductOriginalCost { get; init; }

        public decimal ProductDiscountCost { get; init; }

        public decimal ProductDiscountPercentage { get; init; }
    }
}
