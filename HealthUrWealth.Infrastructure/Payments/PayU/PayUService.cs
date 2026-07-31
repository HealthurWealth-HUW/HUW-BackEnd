using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Payments.Dtos;
using HealthUrWelath.Application.Payments.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace HealthUrWealth.Infrastructure.Payments.PayU
{
    public class PayUService : IPayUService
    {
        private readonly IConfiguration _config;

        public PayUService(IConfiguration config)
        {
            _config = config;
        }

        public PayURequestDto GenerateRequest(CheckoutSummaryDto checkout, long paymentTransactionId)
        {
            var key = _config["PayU:Key"];
            var salt = _config["PayU:Salt"];
            var baseUrl = _config["PayU:BaseUrl"];
            var successUrl = _config["PayU:SuccessUrl"];
            var failureUrl = _config["PayU:FailureUrl"];

            // 🔒 Legacy constants (must match old app)
            const string productInfo = "Order from HealthUrWealth";
            const string serviceProvider = "payu_paisa";

            // ⚠️ IMPORTANT: amount must be invariant culture formatted
            var amountString = checkout.PayableAmount
                .ToString("0.00", CultureInfo.InvariantCulture);

            var hash = GenerateHash(
                key,
                paymentTransactionId.ToString(),
                amountString,
                productInfo,
                checkout.FirstName,
                checkout.Email,
                salt
            );
            var raw = string.Join("|", new[]
                {
                    key,
                    paymentTransactionId.ToString(),
                    amountString,
                    productInfo?.Trim(),
                    checkout.FirstName?.Trim(),
                    checkout.Email?.Trim(),
                    "", // udf1
                    "", // udf2
                    "", // udf3
                    "", // udf4
                    "", // udf5
                    "", "", "", "", "", "" // 6 empty fields
                }) + salt;
            return new PayURequestDto
            {
                Key = key,
                Hash = hash,
                RawHash = raw,
                TxnId = paymentTransactionId.ToString(),
                Amount = checkout.PayableAmount,
                FirstName = checkout.FirstName,
                Email = checkout.Email,
                Phone = checkout.Mobile,
                ProductInfo = productInfo,
                SuccessUrl = successUrl,
                FailureUrl = baseUrl + failureUrl,
                ServiceProvider = serviceProvider
            };
        }

        private string GenerateHash(
            string key,
            string txnId,
            string amount,
            string productInfo,
            string firstName,
            string email,
            string salt)
        {
            //var raw =
            //    $"{key}|{txnId}|{amount}|{productInfo}|{firstName}|{email}||||||||||{salt}";

            var raw = string.Join("|", new[]
                {
                    key,
                    txnId,
                    amount,
                    productInfo?.Trim(),
                    firstName?.Trim(),
                    email?.Trim(),
                    "", // udf1
                    "", // udf2
                    "", // udf3
                    "", // udf4
                    "", // udf5
                    "", "", "", "", "", "" // 6 empty fields
                }) + salt;


            using var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha512.ComputeHash(bytes);

            return BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLower();
        }
    }
}
