using HealthUrWelath.Application.BlueDart.Dtos;
using HealthUrWelath.Application.BlueDart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.BlueDart.Commands
{
    public class GetEDD
    {
        public sealed record Command(string pincode)
            : IRequest<EddDto>;
        public sealed class Handler : IRequestHandler<Command, EddDto>
        {
            private readonly IBluedartService _svc;

            public Handler(IBluedartService svc)
            {
                _svc = svc;
            }

            public Task<EddDto> Handle(Command c, CancellationToken ct)
                => _svc.GetEDD(c.pincode);
        }
    }
}
