namespace HealthUrWealth.Api.Contracts.Impersonation
{
    public sealed record StartImpersonationRequest(
     long CustomerUserId,
     string Reason
 );
}
