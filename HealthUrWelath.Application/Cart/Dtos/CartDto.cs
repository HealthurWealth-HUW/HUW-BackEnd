namespace HealthUrWelath.Application.Cart.Dtos
{
    public sealed class CartDto
    {
        public long CartId { get; init; }
        public IReadOnlyList<CartItemDto> Items { get; init; } = [];
        public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
        public decimal GrandTotalDiscount => Items.Sum(i => i.TotalDiscount);
    }
}
