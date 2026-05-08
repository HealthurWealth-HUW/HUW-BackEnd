using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Home.Queries
{
    public static class GetProductsByCategoryGroup
    {
        public sealed record Query
  : IRequest<IReadOnlyList<CategoryGroupDto>>;
        public sealed class Handler
          : IRequestHandler<Query, IReadOnlyList<CategoryGroupDto>>
        {
            private readonly IDiscoveryReadRepository _repo;
            private readonly IAppCache _cache;

            public Handler(
                IDiscoveryReadRepository repo,
                IAppCache cache)
            {
                _repo = repo;
                _cache = cache;
            }

            public async Task<IReadOnlyList<CategoryGroupDto>> Handle(
                Query request,
                CancellationToken ct)
            {
                return await _cache.GetOrCreateAsync(
                    key: "discovery:home",
                     ttl: TimeSpan.FromMinutes(10),
                    factory: () => _repo.GetCategoryGroupsAsync(8));
            }
        }
    }
}
