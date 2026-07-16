using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Payments.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

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
            private readonly ILogger<Handler> _logger;

            public Handler(IPaymentRepository repository, IUserContext user, ILogger<Handler> logger)
            {
                _repository = repository;
                _user = user;
                _logger = logger;
            }

            public async Task Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var result = await _repository.MarkPaymentFailedAsync(
                    _user.UserId,
                    request.PaymentTransactionId,
                    request.PaymentMode,
                    request.GatewayTransactionId
                );

                if (result is null)
                {
                    // Not an error - duplicate/stale callback or checkout already processed.
                    _logger.LogWarning(
                        "PaymentFailed: no open checkout found for UserId {UserId}, PaymentTransactionId {PaymentTransactionId} - ignoring.",
                        _user.UserId,
                        request.PaymentTransactionId);
                }
            }
        }
    }

}
