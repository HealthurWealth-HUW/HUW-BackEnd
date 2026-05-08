namespace HealthUrWelath.Application.Orders.Dtos
{
    public sealed class OrderSummaryDto
    {
        public long OrderId { get; init; }

        public DateTime OrderDate { get; init; }

        public string PaymentMode { get; init; }

        public string TxnStatus { get; init; }

        public decimal GrandTotal { get; init; }

        public decimal ShippingCharges { get; init; }

        public decimal CGST { get; init; }

        public decimal SGST { get; init; }

        public decimal IGST { get; init; }

        public decimal GST { get; init; }

        public decimal ProductDiscount { get; init; }

        public decimal CouponDiscount { get; init; }

        public decimal TotalDiscount { get; init; }

        public bool IsCartTampered { get; init; }
    }
}
