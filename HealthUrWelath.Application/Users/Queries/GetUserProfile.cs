using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Users.Dtos;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Users.Queries
{
    public class GetUserProfile
    {
        public sealed record Query : IRequest<UserProfileDto>;
        public sealed class Handler : IRequestHandler<Query, UserProfileDto>
        {
            private readonly IUserRepository _repo;
            private readonly IUserContext _currentUser;

            public Handler(IUserRepository repo, IUserContext currentUser)
            {
                _repo = repo;
                _currentUser = currentUser;
            }

            public async Task<UserProfileDto> Handle(Query request, CancellationToken ct)
            {
                var user = await _repo.GetByIdAsync(_currentUser.UserId);

                return user;
            }
        }
    }
}
