namespace HealthUrWelath.Application.Orders.Dtos
{
    public sealed class UserOrderDetailsDto
    {
        public OrderSummaryDto Order { get; init; }

        public ShippingAddressDto ShippingAddress { get; init; }

        public List<OrderItemDto> Items { get; init; }
    }
}
