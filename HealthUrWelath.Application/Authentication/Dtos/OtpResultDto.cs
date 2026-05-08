namespace HealthUrWelath.Application.Authentication.Dtos
{
    public sealed class OtpResultDto
    {
        public bool Sent { get; init; }
        public string MaskedMobile { get; init; } = string.Empty;

    }
}
