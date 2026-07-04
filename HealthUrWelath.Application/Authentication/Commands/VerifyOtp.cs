using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Commands;
using HealthUrWelath.Application.Common.Exceptions;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

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
            private readonly ILogger<VerifyOtpHandler> _logger;

            public VerifyOtpHandler(
                IJwtTokenFactory jwt,
                IUserRepository users,
                IMediator mediator,
                IOtpRepository otp,
                IAppSettings appSettings,
                ILogger<VerifyOtpHandler> logger)
            {
                _jwt = jwt;
                _users = users;
                _mediator = mediator;
                _otp = otp;
                _appSettings = appSettings;
                _logger = logger;
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

                try
                {
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
                        var maxAttempts = int.TryParse(_appSettings?.Get("Otp:MaxVerifyAttempts"), out var configuredMax)
                            ? configuredMax : 3;
                        var lockoutMinutes = int.TryParse(_appSettings?.Get("Otp:LockoutMinutes"), out var configuredLockout)
                            ? configuredLockout : 60;

                        var result = await _otp.ValidateAsync(cmd.Mobile, cmd.Otp, maxAttempts, lockoutMinutes);

                        if (result.IsLocked)
                        {
                            var minutesRemaining = result.LockedUntilUtc.HasValue
                                ? Math.Max(1, (int)Math.Ceiling((result.LockedUntilUtc.Value - DateTime.UtcNow).TotalMinutes))
                                : lockoutMinutes;

                            throw new AppException(
                                $"Too many incorrect attempts. Please try again in {minutesRemaining} minute(s) or contact support.",
                                429);
                        }

                        if (result.UserId == 0)
                        {
                            var message = result.IsExpired
                                ? "OTP Expired, Please Regenerate OTP."
                                : "Please enter correct OTP.";

                            throw new UnauthorizedAccessException(message);
                        }

                        userId = result.UserId;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    throw;
                }
                catch (AppException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "We couldn't verify your OTP right now. Please try again in a moment.",
                        503,
                        ex);
                }

                // Merge guest cart only when not using temporary OTP.
                // A merge failure shouldn't lock the user out of an otherwise valid login.
                if (!isTempOtp && cmd.GuestId.HasValue && cmd.GuestId != Guid.Empty)
                {
                    try
                    {
                        await _mediator.Send(
                            new MergeCart.Command(
                                cmd.GuestId.Value,
                                userId
                            ),
                            ct
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Guest cart merge failed. UserId={UserId} GuestId={GuestId}",
                            userId,
                            cmd.GuestId);
                    }
                }

                // Fetch role if needed
                var role = "Customer";

                try
                {
                    return _jwt.CreateUserToken(userId, role);
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "We couldn't complete your login right now. Please try again.",
                        500,
                        ex);
                }
            }
        }
    }
}
