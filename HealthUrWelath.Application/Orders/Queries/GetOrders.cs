using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Orders.Dtos;
using HealthUrWelath.Application.Orders.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Orders.Queries
{
    public static class GetOrders
    {
        public sealed record Query()
       : IRequest<IReadOnlyList<OrderItemsDto>>;

        public sealed class Handler
            : IRequestHandler<Query, IReadOnlyList<OrderItemsDto>>
        {
            private readonly IOrderRepository _repo;
            private readonly IUserContext _user;

            public Handler(IOrderRepository repo, IUserContext user)
            {
                _repo = repo;
                _user = user;
            }

            public Task<IReadOnlyList<OrderItemsDto>> Handle(Query q, CancellationToken ct)
                => _repo.GetOrders(_user.UserId);
        }
    }
}