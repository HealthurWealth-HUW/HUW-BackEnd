namespace HealthUrWelath.Application.Authentication.Interfaces
{
    public interface IOtpRepository
    {
        Task SaveAsync(long userId, string otp);
        Task<long> ValidateAsync(string mobile, string otp);
    }
}
