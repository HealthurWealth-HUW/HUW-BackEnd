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

        public async Task MarkPaymentFailedAsync(
                long userId,
            long paymentTransactionId,
            string paymentMode,
            string gatewayTxnId)
        {
            await _db.ExecuteAsync(
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
    long checkoutTxnId,
    string gatewayTxnId,
    string paymentMode)
        {
            var result = await _db.QuerySingleAsync<long>(
                "SP_Payment_ConfirmOnlineSuccess",
                new
                {
                    UserId = userId,
                    CheckoutTxnId = checkoutTxnId,
                    GatewayTransactionId = gatewayTxnId,
                    PaymentMode = paymentMode
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
