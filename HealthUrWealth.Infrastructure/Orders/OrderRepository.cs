using Dapper;
using HealthUrWelath.Application.Orders.Dtos;
using HealthUrWelath.Application.Orders.Interfaces;
using System.Data;
using System.Data.Common;

namespace HealthUrWealth.Infrastructure.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _db;

        public OrderRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<OrderItemsDto>> GetOrders(long userId)
        {
            return (await _db.QueryAsync<OrderItemsDto>(
            "SP_UserOrders",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure))
            .ToList();
        }
        public async Task<UserOrderDetailsDto> GetOrdersDetails(long orderId, long userId)
        {
            using var multi = await _db.QueryMultipleAsync(
        "SP_UserOrderDetails",
        new
        {
            PaymentTransactionId = orderId,
            UserId = userId
        },
        commandType: CommandType.StoredProcedure);

            var order = await multi.ReadFirstOrDefaultAsync<OrderSummaryDto>();

            var address = await multi.ReadFirstOrDefaultAsync<ShippingAddressDto>();

            var items = (await multi.ReadAsync<OrderItemDto>()).ToList();

            return new UserOrderDetailsDto
            {
                Order = order,
                ShippingAddress = address,
                Items = items
            };
        }

        public async Task<OrderStatusTimelineDto> GetOrderTimelineAsync(long paymentTransactionId)
        {
            return await _db.QueryFirstOrDefaultAsync<OrderStatusTimelineDto>(
                    "SP_Order_GetStatusTimeline",
                    new { PaymentTransactionId = paymentTransactionId },
                    commandType: CommandType.StoredProcedure);
        }
    }
}
