using FluentValidation;
using HealthUrWelath.Application.Authentication.Commands;

namespace HealthUrWelath.Application.Authentication.Validators
{
    public sealed class VerifyOtpValidator : AbstractValidator<VerifyOtp.Command>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^\d{10}$").WithMessage("Mobile number must be exactly 10 digits.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Matches(@"^\d{6}$").WithMessage("OTP must be a 6-digit number.");
        }
    }
}
