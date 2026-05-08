using HealthUrWelath.Application.Authentication.Dtos;

namespace HealthUrWelath.Application.Authentication.Interfaces
{
    public interface IJwtTokenFactory
    {
        AuthTokenDto CreateUserToken(long userId, string role);
        AuthTokenDto CreateImpersonationToken(long supportUserId, long customerUserId);
    }
}
