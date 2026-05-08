using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Addresses.Queries
{
    public static class GetUserAddresses
    {
        public sealed record Query(long? userAddressId) : IRequest<IReadOnlyList<UserAddressDto>>;

        public sealed class Handler
            : IRequestHandler<Query, IReadOnlyList<UserAddressDto>>
        {
            private readonly IAddressRepository _repo;
            private readonly IUserContext _currentUser;

            public Handler(IAddressRepository repo, IUserContext currentUser)
            {
                _repo = repo;
                _currentUser = currentUser;
            }

            public Task<IReadOnlyList<UserAddressDto>> Handle(
                Query request,
                CancellationToken ct)
            {
                return _repo.GetByUserAsync(_currentUser.UserId, request.userAddressId);
            }
        }
    }

}
