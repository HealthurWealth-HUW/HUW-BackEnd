using HealthUrWelath.Application.Cart.Dtos;
using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Queries
{
    public static class GetCart
    {
        public sealed record Query(long? UserId, Guid? GuestId)
            : IRequest<CartDto>;

        public sealed class Handler
            : IRequestHandler<Query, CartDto>
        {
            private readonly ICartRepository _repo;

            public Handler(ICartRepository repo)
            {
                _repo = repo;
            }

            public async Task<CartDto> Handle(Query q, CancellationToken ct)
            {
                var cartId = await _repo.GetOrCreateCartAsync(q.UserId, q.GuestId);
                var items = await _repo.GetCartItemsAsync(cartId);

                return new CartDto
                {
                    CartId = cartId,
                    Items = items
                };
            }
        }
    }
}
