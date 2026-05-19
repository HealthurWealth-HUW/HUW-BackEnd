using HealthUrWelath.Application.Common.Enums;

namespace HealthUrWelath.Application.Orders.Dtos
{
    public sealed class OrderStatusTimelineDto
    {
        public long PaymentTransactionId { get; init; }

        public DateTime? OrderPlacedDate { get; init; }

        public DateTime? ProcessingDate { get; init; }

        public DateTime? DispatchedDate { get; init; }

        public DateTime? DeliveredDate { get; init; }

        public DateTime? CancelledDate { get; init; }

        public int OrderCurrentStatusId { get; init; }
        public string OrderCurrentStatus => ((OrderStatus)OrderCurrentStatusId).ToString();
    }

}
