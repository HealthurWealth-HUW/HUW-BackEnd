namespace HealthUrWelath.Application.Payments.Dtos
{
    public sealed record PaymentFailedDto(
        long PaymentTransactionId,
        string PaymentMode,
        string GatewayTransactionId
    );
}
