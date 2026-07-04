using Dapper;
using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Checkout.Interfaces;
using System.Data;
using System.Collections.Generic;

public sealed class CheckoutRepository : ICheckoutRepository
{
    private readonly IDbConnection _db;

    public CheckoutRepository(IDbConnection db)
    {
        _db = db;
    }
    public async Task<CheckoutSummaryDto?> GetOpenCheckoutAsync(long userId)
    {
        return await _db.QuerySingleAsync<CheckoutSummaryDto>(
            "SP_Checkout_GetOpen",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );
    }
    public async Task<long> UpsertAndCalculateAsync(
             long userId,
             string paymentMode,

             long billingAddressId,
             long shippingAddressId,
             DateTime expectedDeliveryDate,

             string currencyCode,
             string currencySymbol,
             decimal currencyValue)
    {
            // SQL DateTime minimum is 1753-01-01. Clamp to avoid SqlDateTime overflow when
            // callers pass an uninitialized or out-of-range DateTime.
            var sqlMin = new DateTime(1753, 1, 1);
            var safeExpected = expectedDeliveryDate < sqlMin ? sqlMin : expectedDeliveryDate;

            return await _db.ExecuteScalarAsync<long>(
                "SP_Checkout_UpsertAndCalculate",
                new
                {
                    UserId = userId,
                    PaymentMode = paymentMode,

                    BillingAddressId = billingAddressId,
                    ShippingAddressId = shippingAddressId,
                    ExpectedDeliveryDate = safeExpected,
                    CurrencyCode = currencyCode,
                    CurrencySymbol = currencySymbol,
                    CurrencyValue = currencyValue
                },
                commandType: CommandType.StoredProcedure
            );
    }

    public Task<CheckoutSummaryDto?> GetSummaryAsync(long userId)
        => _db.QueryFirstOrDefaultAsync<CheckoutSummaryDto>(
            "SP_Checkout_GetSummary",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);

    public async Task<IReadOnlyList<CheckoutItemDto>> GetOpenCheckoutItemsAsync(long userId)
    {
        var rows = await _db.QueryAsync<CheckoutItemDto>(
            "SP_Checkout_GetOpenItems",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<long> ConfirmCodAsync(
        long userId, long checkoutTransactionId)
        => await _db.ExecuteScalarAsync<long>(
            "SP_Checkout_ConfirmCOD",
            new
            {
                UserId = userId,
                CheckoutTransactionId = checkoutTransactionId
            },
            commandType: CommandType.StoredProcedure);
}
