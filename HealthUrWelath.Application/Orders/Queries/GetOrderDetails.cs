using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Orders.Dtos;
using HealthUrWelath.Application.Orders.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Orders.Queries
{
    public static class GetOrderDetails
    {
        public sealed record Query(int orderId)
       : IRequest<UserOrderDetailsDto>;
       

        public sealed class Handler
            : IRequestHandler<Query, UserOrderDetailsDto>
        {
            private readonly IOrderRepository _repo;
            private readonly IUserContext _user;
            public Handler(IOrderRepository repo, IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public Task<UserOrderDetailsDto> Handle(Query q, CancellationToken ct)
                => _repo.GetOrdersDetails(q.orderId, _user.UserId);
        }
    }
}