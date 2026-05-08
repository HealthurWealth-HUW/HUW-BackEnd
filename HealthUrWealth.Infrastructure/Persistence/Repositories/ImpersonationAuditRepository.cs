using Dapper;
using HealthUrWelath.Application.Impersonate.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Persistence.Repositories;

public sealed class ImpersonationAuditRepository
    : IImpersonationAuditRepository
{
    private readonly IDbConnection _db;

    public ImpersonationAuditRepository(IDbConnection db)
    {
        _db = db;
    }

    public Task LogStartAsync(
        long supportUserId,
        long customerUserId,
        string reason,
        string ipAddress)
    {
        return _db.ExecuteAsync(
            "SP_Impersonation_Audit_Start",
            new
            {
                SupportUserId = supportUserId,
                CustomerUserId = customerUserId,
                Reason = reason,
                IpAddress = ipAddress
            },
            commandType: CommandType.StoredProcedure);
    }

    public Task LogEndAsync(
        long supportUserId,
        long customerUserId)
    {
        return _db.ExecuteAsync(
            "SP_Impersonation_Audit_End",
            new
            {
                SupportUserId = supportUserId,
                CustomerUserId = customerUserId
            },
            commandType: CommandType.StoredProcedure);
    }
}
