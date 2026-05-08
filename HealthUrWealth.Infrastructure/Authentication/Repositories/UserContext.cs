using HealthUrWelath.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HealthUrWealth.Infrastructure.Authentication.Repositories
{
    public sealed class UserContext : IUserContext
    {        
        private readonly IHttpContextAccessor _accessor;

        public UserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public bool IsAuthenticated =>
           _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public long UserId
        {
            get
            {
                if(!IsAuthenticated)
                    throw new UnauthorizedAccessException("User is not authenticated");

                var userIdClaim = _accessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim))
                    throw new UnauthorizedAccessException("UserId claim missing");

                return long.Parse(userIdClaim);
            }
        }
    }
}
