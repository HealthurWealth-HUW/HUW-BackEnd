using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Addresses.Commands
{
    public static class UpdateAddress
    {
        public sealed record Command(UpdateAddressDto Address) : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly IAddressRepository _repo;
            private readonly IUserContext _currentUser;

            public Handler(IAddressRepository repo, IUserContext currentUser)
            {
                _repo = repo;
                _currentUser = currentUser;
            }

            public async Task Handle(Command request, CancellationToken ct)
            {
                await _repo.UpdateAsync(request.Address, _currentUser.UserId);
            }
        }
    }


}
