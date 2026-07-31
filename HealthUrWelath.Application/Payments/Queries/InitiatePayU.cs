using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Checkout.Interfaces;
using HealthUrWelath.Application.Payments.Dtos;
using HealthUrWelath.Application.Payments.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Payments.Queries
{
    public sealed class InitiatePayU
    {
        public sealed class Query : IRequest<PayURequestDto>;

        public sealed class Handler
            : IRequestHandler<Query, PayURequestDto>
        {
            private readonly ICheckoutRepository _checkoutRepo;
            private readonly IPaymentRepository _paymentRepo;
            private readonly IPayUService _payUService;
            private readonly IUserContext _user;

            public Handler(
                ICheckoutRepository checkoutRepo,
                IPaymentRepository paymentRepo,
                IPayUService payUService,
                IUserContext user)
            {
                _checkoutRepo = checkoutRepo;
                _paymentRepo = paymentRepo;
                _payUService = payUService;
                _user = user;
            }

            public async Task<PayURequestDto> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var checkout = await _checkoutRepo.GetOpenCheckoutAsync(_user.UserId);

                if (checkout == null)
                    throw new Exception("No open checkout found");

                // Create the durable Pending PaymentTransactions row before handing off
                // to the gateway - its id, not the checkout snapshot's, is what PayU
                // gets back on success/failure. See SP_Payment_CreatePendingFromCheckout.
                var paymentTransactionId = await _paymentRepo.CreatePendingFromCheckoutAsync(
                    _user.UserId, checkout.PaymentTransactionId);

                return _payUService.GenerateRequest(checkout, paymentTransactionId);
            }
        }
    }
}
