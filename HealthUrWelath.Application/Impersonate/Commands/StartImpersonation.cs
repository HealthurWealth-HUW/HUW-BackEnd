using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Impersonate.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Impersonate.Commands;
public static class StartImpersonation
{
    public sealed record Command(
        long SupportUserId,
        long CustomerUserId,
        string Reason,
        string IpAddress
    ) : IRequest<AuthTokenDto>;

    public sealed class Handler
        : IRequestHandler<Command, AuthTokenDto>
    {
        private readonly IImpersonationAuditRepository _auditRepo;
        private readonly IJwtTokenFactory _jwt;

        public Handler(
            IImpersonationAuditRepository auditRepo,
            IJwtTokenFactory jwt)
        {
            _auditRepo = auditRepo;
            _jwt = jwt;
        }

        public async Task<AuthTokenDto> Handle(
            Command cmd,
            CancellationToken ct)
        {
            await _auditRepo.LogStartAsync(
                cmd.SupportUserId,
                cmd.CustomerUserId,
                cmd.Reason,
                cmd.IpAddress);

            return _jwt.CreateImpersonationToken(
                cmd.SupportUserId,
                cmd.CustomerUserId);
        }
    }
}
