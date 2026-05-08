using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Commands;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Authentication.Commands
{
    public static class VerifyOtp
    {
        public sealed record Command(
    string Mobile,
    string Otp,
        Guid? GuestId
) : IRequest<AuthTokenDto>;

        public sealed class VerifyOtpHandler
       : IRequestHandler<Command, AuthTokenDto>
        {
            private readonly IJwtTokenFactory _jwt;
            private readonly IUserRepository _users;
            private readonly IMediator _mediator; // reuse CQRS
            private readonly IOtpRepository _otp;

            public VerifyOtpHandler(
                IJwtTokenFactory jwt,
                IUserRepository users,
                IMediator mediator,
                IOtpRepository otp)
            {
                _jwt = jwt;
                _users = users;
                _mediator = mediator;
                _otp = otp;
            }

            public async Task<AuthTokenDto> Handle(
                Command cmd,
                CancellationToken ct)
            {
                var userId = await _otp.ValidateAsync(cmd.Mobile, cmd.Otp);
                if (userId == 0)
                    throw new UnauthorizedAccessException("Invalid or expired OTP");


                if (cmd.GuestId.HasValue && cmd.GuestId != Guid.Empty)
                {
                    await _mediator.Send(
                        new MergeCart.Command(
                            cmd.GuestId.Value,
                            userId
                        ),
                        ct
                    );

                }
                // Fetch role if needed
                var role = "Customer";

                return _jwt.CreateUserToken(userId, role);
            }
        }
    }
}
