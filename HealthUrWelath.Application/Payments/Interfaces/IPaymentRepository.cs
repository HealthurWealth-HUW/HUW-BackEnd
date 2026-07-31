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
       long paymentTransactionId,
       string gatewayTxnId,
       string paymentMode);

        // Creates the durable PaymentTransactions row (PaymentStatus = Pending) at the
        // moment a checkout is handed off to the payment gateway - see
        // SP_Payment_CreatePendingFromCheckout for why this exists.
        Task<long> CreatePendingFromCheckoutAsync(long userId, long checkoutTransactionId);
    }
}
