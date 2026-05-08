namespace HealthUrWelath.Application.Cart.Dtos
{
    public sealed class CartItemDto
    {
        public long ProductId { get; init; }
        public string ProductName { get; init; }
        public string ProductImgUrl { get; init; }

        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
        public decimal DiscountPercentage { get; init; }
        public decimal DiscountPerUnit { get; init; }
        public decimal TotalDiscount { get; init; }

    }
}
