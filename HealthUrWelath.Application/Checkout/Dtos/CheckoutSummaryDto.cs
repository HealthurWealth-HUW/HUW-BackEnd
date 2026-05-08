namespace HealthUrWelath.Application.Checkout.Dtos
{
    public sealed class CheckoutSummaryDto
    {
        public long PaymentTransactionId { get; init; }

        public decimal OrderAmount { get; init; }
        public decimal PayableAmount { get; init; }

        public decimal TxnAmount { get; init; }
        public decimal ShippingCharges { get; init; }

        public string CurrencyCode { get; init; } = default!;
        public string CurrencySymbol { get; init; } = default!;
        public decimal CurrencyValue { get; init; }

        public string PaymentMode { get; init; } = default!;
        public string? TxnStatus { get; init; }
        public string? OrderStatus { get; init; }

        public long? Promo_Code_ID { get; init; }
        public decimal? Promo_Code_Amount { get; init; }

        // 🔥 Required for payment gateway
        public string FirstName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Mobile { get; init; }
    }
}
