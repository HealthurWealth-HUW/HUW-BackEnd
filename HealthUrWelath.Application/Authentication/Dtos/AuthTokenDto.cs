namespace HealthUrWelath.Application.Authentication.Dtos
{
    public sealed class AuthTokenDto
    {
        public string AccessToken { get; init; } = default!;
        public DateTime ExpiresAt { get; init; }
    }
}
