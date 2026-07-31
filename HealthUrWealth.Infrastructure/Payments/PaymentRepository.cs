using Dapper;
using HealthUrWelath.Application.Payments.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _db;

        public PaymentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<long?> MarkPaymentFailedAsync(
                long userId,
            long paymentTransactionId,
            string paymentMode,
            string gatewayTxnId)
        {
            return await _db.QuerySingleOrDefaultAsync<long?>(
                "SP_Payment_MarkFailed",
                new
                {
                    UserId = userId,
                    PaymentTransactionId = paymentTransactionId,
                    PaymentMode = paymentMode,
                    PGTxnId = gatewayTxnId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<long> ConfirmOnlinePaymentAsync(
    long userId,
    long paymentTransactionId,
    string gatewayTxnId,
    string paymentMode)
        {
            var result = await _db.QuerySingleAsync<long>(
                "SP_Payment_ConfirmOnlineSuccess",
                new
                {
                    UserId = userId,
                    PaymentTransactionId = paymentTransactionId,
                    GatewayTransactionId = gatewayTxnId,
                    PaymentMode = paymentMode
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<long> CreatePendingFromCheckoutAsync(long userId, long checkoutTransactionId)
        {
            return await _db.QuerySingleAsync<long>(
                "SP_Payment_CreatePendingFromCheckout",
                new
                {
                    UserId = userId,
                    CheckoutTransactionId = checkoutTransactionId
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
