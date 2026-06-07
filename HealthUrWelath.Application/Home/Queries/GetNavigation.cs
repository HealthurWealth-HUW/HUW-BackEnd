using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using MediatR;

namespace HealthUrWealth.Application.Home.Queries;

public static class GetNavigation
{
    public sealed record Query
        : IRequest<IReadOnlyList<NavigationDto>>;

    public sealed class Handler
        : IRequestHandler<Query, IReadOnlyList<NavigationDto>>
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

        public async Task<IReadOnlyList<NavigationDto>> Handle(
            Query request,
            CancellationToken ct)
        {
            return await _cache.GetOrCreateAsync(
                featureName: "Navigation",
                key: "navigation:v1",
                factory: () => _repository.GetNavigationAsync());
        }
    }
}