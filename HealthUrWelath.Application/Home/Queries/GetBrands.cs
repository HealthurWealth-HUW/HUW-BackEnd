using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Home.Queries;

public static class GetBrands
{
    public sealed record Query
        : IRequest<IReadOnlyList<BrandGroupDto>>;

    public sealed class Handler
        : IRequestHandler<Query, IReadOnlyList<BrandGroupDto>>
    {
        private readonly INavigationReadRepository _repository;
        private readonly IAppCache _cache;

        public Handler(
            INavigationReadRepository repository,
            IAppCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IReadOnlyList<BrandGroupDto>> Handle(
            Query request,
            CancellationToken ct)
        {
            return await _cache.GetOrCreateAsync(
                key: "brands:v1",
                ttl: TimeSpan.FromMinutes(60),
                factory: () => _repository.GetBrandsAsync());
        }
    }
}