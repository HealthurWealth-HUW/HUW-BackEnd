using HealthUrWelath.Application.Authentication.Interfaces;
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

            public Handler(
                ICheckoutRepository repo,
                IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public async Task<CheckoutSummaryDto> Handle(
                Query request,
                CancellationToken ct)
            {
                return await _repo.GetSummaryAsync(_user.UserId)
                    ?? throw new AppException("No checkout in progress.", 404);
            }
        }

    }
}
