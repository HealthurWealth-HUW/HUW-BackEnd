namespace HealthUrWelath.Application.Payments.Dtos
{
    public sealed record PaymentSuccessRequestDto(
        long PaymentTransactionId,
    string GatewayTransactionId,
    string PaymentMode
        );
}
