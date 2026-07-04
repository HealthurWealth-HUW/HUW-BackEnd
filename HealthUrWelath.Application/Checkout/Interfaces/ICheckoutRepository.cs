using HealthUrWelath.Application.Checkout.Dtos;

namespace HealthUrWelath.Application.Checkout.Interfaces
{
    public interface ICheckoutRepository
    {
        Task<CheckoutSummaryDto?> GetOpenCheckoutAsync(long userId);

        Task<long> UpsertAndCalculateAsync(
             long userId,
             string paymentMode,

             long billingAddressId,
             long shippingAddressId,
             DateTime expectedDeliveryDate,

             string currencyCode,
             string currencySymbol,
             decimal currencyValue);
        Task<CheckoutSummaryDto?> GetSummaryAsync(long userId);

        Task<IReadOnlyList<CheckoutItemDto>> GetOpenCheckoutItemsAsync(long userId);

        Task<long> ConfirmCodAsync(long userId, long checkoutTransactionId);
    }
}
