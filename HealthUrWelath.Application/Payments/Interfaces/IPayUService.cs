using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Payments.Dtos;

namespace HealthUrWelath.Application.Payments.Interfaces
{
    public interface IPayUService
    {
        PayURequestDto GenerateRequest(CheckoutSummaryDto checkout, long paymentTransactionId);
    }
}
