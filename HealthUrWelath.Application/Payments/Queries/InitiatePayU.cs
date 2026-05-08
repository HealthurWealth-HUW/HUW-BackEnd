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
            private readonly IPayUService _payUService;
            private readonly IUserContext _user;

            public Handler(
                ICheckoutRepository checkoutRepo,
                IPayUService payUService,
                IUserContext user)
            {
                _checkoutRepo = checkoutRepo;
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

                return _payUService.GenerateRequest(checkout);
            }
        }
    }
}
