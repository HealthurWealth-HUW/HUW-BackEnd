using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;
using System.Data;

namespace HealthUrWelath.Application.Users.Commands
{
    public static class UpdateUserProfile
    {
        public sealed record Command(
            string FirstName,
            string LastName,
            string EmailId,
           // string MobileNo,
            string alternateMobileNo
        ) : IRequest;

        internal sealed class Handler
                : IRequestHandler<Command>
        {
            private readonly IDbConnection _db;
            private readonly IUserContext _currentUser;
            private readonly IUserRepository _userRepository;

            public Handler(
                IDbConnection db,
                IUserContext currentUser, IUserRepository userRepository)
            {
                _db = db;
                _currentUser = currentUser; 
                _userRepository = userRepository;

            }

            public async Task Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                await _userRepository.UpdateProfileAsync(
                _currentUser.UserId,
                request.FirstName,
                request.LastName,
                request.EmailId,
               // request.MobileNo,
                request.alternateMobileNo,
                cancellationToken);
            }
        }

    }
}
