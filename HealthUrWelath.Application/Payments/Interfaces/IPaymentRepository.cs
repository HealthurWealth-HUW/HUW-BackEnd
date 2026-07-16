namespace HealthUrWelath.Application.Payments.Interfaces
{
    public interface IPaymentRepository
    {
        Task<long?> MarkPaymentFailedAsync(
            long userId,
            long paymentTransactionId,
            string paymentMode,
            string gatewayTxnId
        );
        Task<long> ConfirmOnlinePaymentAsync(
       long userId,
       long checkoutTxnId,
       string gatewayTxnId,
       string paymentMode);
    }
}
