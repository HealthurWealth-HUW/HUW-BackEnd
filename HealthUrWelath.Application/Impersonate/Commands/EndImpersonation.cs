using HealthUrWelath.Application.Impersonate.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Impersonate.Commands;

public static class EndImpersonation
{
    public sealed record Command(
        long SupportUserId,
        long CustomerUserId
    ) : IRequest;

    public sealed class Handler
        : IRequestHandler<Command>
    {
        private readonly IImpersonationAuditRepository _auditRepo;

        public Handler(IImpersonationAuditRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        public async Task Handle(
            Command cmd,
            CancellationToken ct)
        {
            await _auditRepo.LogEndAsync(
                cmd.SupportUserId,
                cmd.CustomerUserId);
        }
    }
}
