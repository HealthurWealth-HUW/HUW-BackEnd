using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class RemoveCoupon
    {
        public sealed record Command(long CartId) : IRequest;

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
                var userId = _userContext.UserId;

                await _repo.RemoveCouponAsync(q.CartId, userId);
            }
        }
    }
}
