namespace HealthUrWealth.Application.Impersonate.Dtos;

public sealed class ImpersonateRequestDto
{
    public long CustomerUserId { get; init; }
    public string Reason { get; init; } = default!;
}
