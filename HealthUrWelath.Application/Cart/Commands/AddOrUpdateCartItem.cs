using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class AddOrUpdateCartItem
    {
        public sealed record Command(
            long? UserId,
            Guid? GuestId,
            long ProductId,
            int Quantity)
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
                await _repo.AddOrUpdateItemAsync(cartId, c.ProductId, c.Quantity);
            }
        }
    }
}
