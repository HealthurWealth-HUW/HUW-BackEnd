using HealthUrWelath.Application.Orders.Dtos;
namespace HealthUrWelath.Application.Orders.Interfaces
{
    public interface IOrderRepository
    {
        public  Task<IReadOnlyList<OrderItemsDto>> GetOrders(long userId);
        public Task<UserOrderDetailsDto> GetOrdersDetails(long orderId, long userId);

        Task<OrderStatusTimelineDto> GetOrderTimelineAsync(long paymentTransactionId, long userId);
    }
}