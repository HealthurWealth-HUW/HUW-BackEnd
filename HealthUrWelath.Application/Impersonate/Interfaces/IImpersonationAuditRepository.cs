namespace HealthUrWelath.Application.Impersonate.Interfaces
{
    public interface IImpersonationAuditRepository
    {
        Task LogStartAsync(
            long supportUserId,
            long customerUserId,
            string reason,
            string ipAddress);

        Task LogEndAsync(
            long supportUserId,
            long customerUserId);
    }

}
