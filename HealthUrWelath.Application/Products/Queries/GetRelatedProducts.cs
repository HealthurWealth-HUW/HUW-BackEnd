using HealthUrWelath.Application.Home.Dtos;
using MediatR;

namespace HealthUrWelath.Application.Products.Queries
{
    public class GetRelatedProducts
    {
        public sealed record Query(long ProductId)
            : IRequest<IReadOnlyList<ProductSummaryDto>>;

        public sealed class Handler
            : IRequestHandler<Query, IReadOnlyList<ProductSummaryDto>>
        {
            private readonly IProductReadRepository _repo;

            public Handler(IProductReadRepository repo)
            {
                _repo = repo;
            }

            public async Task<IReadOnlyList<ProductSummaryDto>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                return await _repo.GetRelatedProductsAsync(request.ProductId);
            }
        }
    }
}
