using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Orders.Dtos;
using HealthUrWelath.Application.Orders.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Orders.Queries
{
    public class GetOrderTimeline
    {
        public sealed record Query(long PaymentTransactionId)
            : IRequest<OrderStatusTimelineDto>;

        public sealed class Handler
            : IRequestHandler<Query, OrderStatusTimelineDto>
        {
            private readonly IOrderRepository _repo;
            private readonly IUserContext _user;

            public Handler(IOrderRepository repo, IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public async Task<OrderStatusTimelineDto> Handle(
                Query request,
                CancellationToken ct)
            {
                return await _repo.GetOrderTimelineAsync(
                    request.PaymentTransactionId,
                    _user.UserId);
            }
        }
    }
}
