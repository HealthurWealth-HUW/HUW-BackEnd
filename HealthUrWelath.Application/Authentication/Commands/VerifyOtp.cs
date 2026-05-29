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
            private readonly IAppSettings _appSettings;

            public VerifyOtpHandler(
                IJwtTokenFactory jwt,
                IUserRepository users,
                IMediator mediator,
                IOtpRepository otp,
                IAppSettings appSettings)
            {
                _jwt = jwt;
                _users = users;
                _mediator = mediator;
                _otp = otp;
                _appSettings = appSettings;
            }

            public async Task<AuthTokenDto> Handle(
                Command cmd,
                CancellationToken ct)
            {
                long userId = 0;
                var isTempOtp = false;

                // Check for configured temporary OTP first (useful for support/testing)
                var tempOtpCfg = _appSettings?.Get("TempOtp:SupportOtp") ?? string.Empty;

                var allowedOtps = tempOtpCfg.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s.Trim()).ToArray();

                if (allowedOtps.Any(s => string.Equals(s, cmd.Otp, StringComparison.OrdinalIgnoreCase)))
                {
                    var uid = await _users.GetUserIdByMobileAsync(cmd.Mobile);
                    if (!uid.HasValue)
                        throw new UnauthorizedAccessException("Invalid or expired OTP");

                    userId = uid.Value;
                    isTempOtp = true;
                }
                else
                {
                    // Fall back to normal OTP validation
                    userId = await _otp.ValidateAsync(cmd.Mobile, cmd.Otp);
                }

                if (userId == 0)
                    throw new UnauthorizedAccessException("Invalid or expired OTP");


                // Merge guest cart only when not using temporary OTP
                if (!isTempOtp && cmd.GuestId.HasValue && cmd.GuestId != Guid.Empty)
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
