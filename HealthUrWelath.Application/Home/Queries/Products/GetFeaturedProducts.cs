using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Home.Queries.Products
{
    public class GetFeaturedProducts
    {
        public sealed record Query
        : IRequest<IReadOnlyList<ProductSummaryDto>>;

        public sealed class Handler
        : IRequestHandler<Query, IReadOnlyList<ProductSummaryDto>>
        {
            private readonly IProductReadRepository _repository;
            private readonly IAppCache _cache;

            public Handler(
                IProductReadRepository repository,
                IAppCache cache)
            {
                _repository = repository;
                _cache = cache;
            }

            public async Task<IReadOnlyList<ProductSummaryDto>> Handle(
                Query request,
                CancellationToken ct)
            {
                return await _cache.GetOrCreateAsync(
                    key: "discovery:featured:v1",
                    ttl: TimeSpan.FromMinutes(10),
                    factory: () => _repository.GetFeaturedProductsAsync(8));
            }
        }
    }
}
