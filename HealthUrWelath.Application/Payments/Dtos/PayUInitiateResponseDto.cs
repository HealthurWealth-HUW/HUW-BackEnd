namespace HealthUrWelath.Application.Payments.Dtos
{
    public record PayUInitiateResponseDto(
    string Url,
    Dictionary<string, string> FormFields
);
}
