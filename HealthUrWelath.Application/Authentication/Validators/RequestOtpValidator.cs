using FluentValidation;
using HealthUrWelath.Application.Authentication.Commands;
using System.Text.RegularExpressions;

namespace HealthUrWelath.Application.Authentication.Validators
{
    public sealed class RequestOtpValidator : AbstractValidator<RequestOtp.Command>
    {
        public RequestOtpValidator()
        {
            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Must(mobile => Regex.IsMatch(mobile, @"^\d{10}$"))
                .WithMessage("Mobile number must be exactly 10 digits.");
        }
    }
}
