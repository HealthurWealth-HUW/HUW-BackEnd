using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Common.Exceptions;
using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Application.Notifications.Templates;
using HealthUrWelath.Application.Users.Dtos;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace HealthUrWelath.Application.Authentication.Commands
{
    public sealed class RequestOtp
    {
        public sealed record Command(string Mobile)
            : IRequest<OtpResultDto>;

        public sealed class Handler
            : IRequestHandler<Command, OtpResultDto>
        {
            private readonly IUserRepository _users;
            private readonly IOtpRepository _otp;
            private readonly INotificationService _notify;

            public Handler(
                IUserRepository users,
                IOtpRepository otp,
                INotificationService notify)
            {
                _users = users;
                _otp = otp;
                _notify = notify;
            }

            public async Task<OtpResultDto> Handle(
                Command cmd,
                CancellationToken ct)
            {
                // 1️) Normalize input
                var (mobile, email) = Parse(cmd.Mobile);

                // 2️) Get or create user (OTP-first rule)
                UserDto user;
                try
                {
                    user = await _users.GetOrCreateByMobileAsync(mobile, email);
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "We couldn't process your request right now. Please try again in a moment.",
                        503,
                        ex);
                }

                // 3️) Generate OTP
                var otp = RandomNumberGenerator
                    .GetInt32(0, 1_000_000)
                    .ToString("D6");

                // 4️) Persist OTP
                try
                {
                    await _otp.SaveAsync(user.UserId, otp);
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "Unable to generate an OTP at this time. Please try again.",
                        503,
                        ex);
                }

                // 5️) Notify user
                var sent = await _notify.SendAsync(new NotificationRequest
                {
                    EmailTemplateKey = EmailTemplateKeys.OtpLogin,
                    SmsTemplateKey = SmsTemplateKeys.OtpLogin,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    Channel = NotificationChannel.Both,
                    Tokens =
                    {
                        ["FirstName"] = user.FirstName,
                        ["OTP"] = otp
                    }
                });

                if (!sent)
                    throw new AppException(
                        "We couldn't send the OTP right now. Please try again in a moment.",
                        502);

                return new OtpResultDto
                {
                    Sent = true,
                    MaskedMobile = Mask(mobile)
                };
            }

            // ----------------- helpers -----------------

            private static (string mobile, string? email) Parse(string input)
            {
                if (input.Contains("@"))
                    return (string.Empty, input);

                return (input, null);
            }

            private static string Mask(string mobile)
            {
                if (mobile.Length < 4) return "****";
                return $"******{mobile[^4..]}";
            }
        }
    }
}
