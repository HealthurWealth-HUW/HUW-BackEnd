using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Payments.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Payments.Commands
{
    public class PaymentFailed
    {
        public sealed record Command(
            long PaymentTransactionId,
            string PaymentMode,
            string GatewayTransactionId
        ) : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly IPaymentRepository _repository;
            private readonly IUserContext _user;
            public Handler(IPaymentRepository repository,IUserContext user)
            {
                _repository = repository;
                _user = user;
            }

            public async Task Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                await _repository.MarkPaymentFailedAsync(
                    _user.UserId,
                    request.PaymentTransactionId,
                    request.PaymentMode,
                    request.GatewayTransactionId
                );
            }
        }
    }

}
