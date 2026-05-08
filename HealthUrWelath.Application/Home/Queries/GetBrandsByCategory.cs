using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Home.Queries;

public static class GetBrandsByCategory
{
    public sealed record Query(int CategoryId) : IRequest<IReadOnlyList<BrandGroupDto>>;

    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<BrandGroupDto>>
    {
        private readonly INavigationReadRepository _repository;
        private readonly IAppCache _cache;

        public Handler(INavigationReadRepository repository, IAppCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IReadOnlyList<BrandGroupDto>> Handle(Query request, CancellationToken ct)
        {
            var key = $"brands:category:{request.CategoryId}:v1";

            return await _cache.GetOrCreateAsync(
                key: key,
                ttl: TimeSpan.FromMinutes(60),
                factory: () => _repository.GetBrandsByCategoryAsync(request.CategoryId));
        }
    }
}
