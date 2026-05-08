using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Authentication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthUrWealth.Infrastructure.Authentication.Jwt
{
    public sealed class JwtTokenFactory : IJwtTokenFactory
    {
        private const int TokenExpiryHours = 1;
        private readonly IConfiguration _config;

        public JwtTokenFactory(IConfiguration config)
        {
            _config = config;
        }

        public AuthTokenDto CreateUserToken(long userId, string role)
        {
            var claims = BuildBaseClaims(userId, role);
            return CreateToken(claims);
        }

        public AuthTokenDto CreateImpersonationToken(
            long supportUserId,
            long customerUserId)
        {
            var claims = BuildBaseClaims(customerUserId, "Customer");

            claims.Add(new Claim("impersonated", "true"));
            claims.Add(new Claim("supportUserId", supportUserId.ToString()));

            return CreateToken(claims);
        }

        private static List<Claim> BuildBaseClaims(long userId, string role)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
        }

        private AuthTokenDto CreateToken(IEnumerable<Claim> claims)
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Jwt:Key is missing");

            var signingKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(TokenExpiryHours),
                signingCredentials: credentials
            );

            return new AuthTokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
        }
    }
}
