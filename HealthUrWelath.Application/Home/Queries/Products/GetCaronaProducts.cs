using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Home.Queries.Products
{
    public static class GetCaronaProducts
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

            public Task<IReadOnlyList<ProductSummaryDto>> Handle(
                Query request,
                CancellationToken ct)
            {
                return _cache.GetOrCreateAsync(
                    key: "discovery:carona:v1",
                    ttl: TimeSpan.FromMinutes(10),
                    factory: () => _repository.GetCaronaProductsAsync(12));
            }
        }
    }

}
