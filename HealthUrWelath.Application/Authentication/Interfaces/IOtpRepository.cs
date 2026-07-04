using HealthUrWelath.Application.Authentication.Dtos;

namespace HealthUrWelath.Application.Authentication.Interfaces
{
    public interface IOtpRepository
    {
        Task SaveAsync(long userId, string otp);
        Task<OtpValidationResult> ValidateAsync(string mobile, string otp, int maxAttempts, int lockoutMinutes);
    }
}
