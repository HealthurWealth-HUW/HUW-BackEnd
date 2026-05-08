using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public class UpdateCartQuantity
    {
        public record Command(long ProductId, int Quantity) : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly ICartRepository _repo;
            private readonly IUserContext _user;

            public Handler(ICartRepository repo, IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                await _repo.UpdateQuantityAsync(
                    _user.UserId,
                    request.ProductId,
                    request.Quantity);
            }
        }
    }
}
