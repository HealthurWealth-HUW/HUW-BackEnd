using HealthUrWelath.Application.Products.Dto;
using MediatR;

namespace HealthUrWelath.Application.Products.Queries
{
    public class GetSubProducts
    {
        public sealed record Query(long ProductId)
            : IRequest<IReadOnlyList<SubProductDto>>;

        public sealed class Handler
            : IRequestHandler<Query, IReadOnlyList<SubProductDto>>
        {
            private readonly IProductReadRepository _repo;

            public Handler(IProductReadRepository repo)
            {
                _repo = repo;
            }

            public async Task<IReadOnlyList<SubProductDto>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                return await _repo.GetSubProductsAsync(request.ProductId);
            }
        }
    }
}
