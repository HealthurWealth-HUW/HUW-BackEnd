namespace HealthUrWelath.Application.Payments.Dtos
{
    public sealed class PayURequestDto
    {
        public string Key { get; init; } = default!;
        public string Hash { get; init; } = default!;
        public string TxnId { get; init; } = default!;
        public decimal Amount { get; init; }

        public string FirstName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;

        public string ProductInfo { get; init; }

        public string SuccessUrl { get; init; } = default!;
        public string FailureUrl { get; init; } = default!;

        public string ServiceProvider { get; init; }

        // optional if needed later
        public string? Udf1 { get; init; }
        public string? Udf2 { get; init; }
        public string? Udf3 { get; init; }
        public string? Udf4 { get; init; }
        public string? Udf5 { get; init; }
        public string RawHash { get; set; }
    }
}
