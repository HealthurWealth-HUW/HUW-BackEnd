using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Users.Dtos;

namespace HealthUrWelath.Application.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetOrCreateByMobileAsync(string mobile, string? email);
        Task<long?> GetUserIdByMobileAsync(string mobile);
        Task<bool> IsSupportAgentAsync(long userId);

        Task<UserProfileDto> GetByIdAsync(long userId);

        Task UpdateProfileAsync(
            long userId,
            string firstName,
            string lastName,
           // string emailId,
            string mobileNo,
            string alternateMobileNo,
            CancellationToken cancellationToken = default);
    }
}
