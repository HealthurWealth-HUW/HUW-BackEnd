using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Checkout.Commands;
using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Checkout.Interfaces;
using HealthUrWelath.Application.Common.Exceptions;
using MediatR;

namespace HealthUrWelath.Application.Checkout.Queries
{
    public static class GetCheckoutSummary
    {
        public sealed record Query
    : IRequest<CheckoutSummaryDto>;

        public sealed class Handler
    : IRequestHandler<Query, CheckoutSummaryDto>
        {
            private readonly ICheckoutRepository _repo;
            private readonly IUserContext _user;
            private readonly IAddressRepository _addresses;
            private readonly IMediator _mediator;

            public Handler(
                ICheckoutRepository repo,
                IUserContext user,
                IAddressRepository addresses,
                IMediator mediator)
            {
                _repo = repo;
                _user = user;
                _addresses = addresses;
                _mediator = mediator;
            }

            public async Task<CheckoutSummaryDto> Handle(
                Query request,
                CancellationToken ct)
            {
                var summary = await _repo.GetSummaryAsync(_user.UserId);
                if (summary is not null)
                    return summary;

                // No checkout has been started yet (e.g. the client called summary directly
                // from the cart page without calling POST /checkout/start first). Rather than
                // 404ing, auto-start one from the user's cart and most recently used address
                // so the checkout page always has something to show.
                var addresses = await _addresses.GetByUserAsync(_user.UserId, null);
                var address = addresses.FirstOrDefault()
                    ?? throw new AppException("No checkout in progress. Please add a shipping address to continue.", 404);

                await _mediator.Send(
                    new StartCheckout.Command(
                        new StartCheckoutDto("Cash On Delivery", address.UserAddressId, address.UserAddressId)),
                    ct);

                return await _repo.GetSummaryAsync(_user.UserId)
                    ?? throw new AppException("No checkout in progress.", 404);
            }
        }

    }
}
