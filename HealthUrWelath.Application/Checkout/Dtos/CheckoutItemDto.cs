namespace HealthUrWelath.Application.Checkout.Dtos
{
    public sealed class CheckoutItemDto
    {
        public long ProductId { get; init; }
        public string ProductName { get; init; } = default!;
        public string? ProductImgUrl { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
        public decimal DiscountPercentage { get; init; }
        public decimal DiscountPerUnit { get; init; }
    }
}
