using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class RemoveCartItem
    {
        public sealed record Command(long? UserId, Guid? GuestId, long ProductId)
            : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly ICartRepository _repo;

            public Handler(ICartRepository repo)
            {
                _repo = repo;
            }

            public async Task Handle(Command c, CancellationToken ct)
            {
                var cartId = await _repo.GetOrCreateCartAsync(c.UserId, c.GuestId);
                await _repo.RemoveItemAsync(cartId, c.ProductId);
            }
        }
    }
}
