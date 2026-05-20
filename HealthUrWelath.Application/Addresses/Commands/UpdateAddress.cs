using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Common.Exceptions;
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
                // If UI didn't provide AddressType (0), default to Billing (1)
                var addr = request.Address;
                if (addr.AddressTypeId == 0)
                {
                    addr = addr with { AddressTypeId = 1 };
                }

                // Ensure AddressType is valid (Billing = 1, Shipping = 2)
                if (addr.AddressTypeId != 1 && addr.AddressTypeId != 2)
                    throw new AppException("AddressType is required and must be Billing(1) or Shipping(2)");

                await _repo.UpdateAsync(addr, _currentUser.UserId);
            }
        }
    }


}
