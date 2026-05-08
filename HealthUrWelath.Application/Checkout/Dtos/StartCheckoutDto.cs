namespace HealthUrWelath.Application.Checkout.Dtos
{
    public sealed record StartCheckoutDto(
    string PaymentMode,

    long BillingAddressId,
    long ShippingAddressId

    //string CurrencyCode,
    //string CurrencySymbol,
    //decimal CurrencyValue
    );

    public sealed record CheckoutResultDto(
        long PaymentTransactionId
    );

    public sealed record ConfirmCheckoutRequestDto(
       long paymentTransactionId
   );
}
