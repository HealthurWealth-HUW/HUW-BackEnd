using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Checkout.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Checkout.Queries
{
    public static class GetCheckoutItems
    {
        public sealed record Query : IRequest<IReadOnlyList<CheckoutItemDto>>;

        public sealed class Handler : IRequestHandler<Query, IReadOnlyList<CheckoutItemDto>>
        {
            private readonly ICheckoutRepository _repo;
            private readonly IUserContext _user;

            public Handler(ICheckoutRepository repo, IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public Task<IReadOnlyList<CheckoutItemDto>> Handle(Query request, CancellationToken ct)
                => _repo.GetOpenCheckoutItemsAsync(_user.UserId);
        }
    }
}
