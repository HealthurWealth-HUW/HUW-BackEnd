using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class RemoveCoupon
    {
        public sealed record Command : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly ICartRepository _repo;
            private readonly IUserContext _userContext;

            public Handler(ICartRepository repo, IUserContext userContext)
            {
                _repo = repo;
                _userContext = userContext;
            }

            public async Task Handle(Command q, CancellationToken ct)
            {
                // Resolve the caller's own cart server-side — never trust a client-supplied CartId here.
                var cartId = await _repo.GetOrCreateCartAsync(_userContext.UserId, null);

                await _repo.RemoveCouponAsync(cartId, _userContext.UserId);
            }
        }
    }
}
