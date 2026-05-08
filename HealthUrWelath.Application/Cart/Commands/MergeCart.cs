using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class MergeCart
    {
        public sealed record Command(Guid GuestId, long UserId)
            : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly ICartRepository _repo;

            public Handler(ICartRepository repo)
            {
                _repo = repo;
            }

            public Task Handle(Command c, CancellationToken ct)
                => _repo.MergeGuestCartAsync(c.GuestId, c.UserId);
        }
    }
}
