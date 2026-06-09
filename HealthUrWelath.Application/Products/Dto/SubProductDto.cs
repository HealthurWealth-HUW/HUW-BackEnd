namespace HealthUrWelath.Application.Products.Dto
{
    public sealed record SubProductDto
    {
        public long ProductId { get; init; }
        public long SubProductId { get; init; }
        public int Quantity { get; init; }

        public string SpName { get; init; }
        public decimal SubProductOriginalCost { get; init; }
        public decimal SubProductDiscountPercentage { get; init; }
        public decimal SubProductDiscountCost { get; init; }
        public decimal SubProductCost { get; init; }
        public int SubProductQuantity { get; init; }
    }
}
