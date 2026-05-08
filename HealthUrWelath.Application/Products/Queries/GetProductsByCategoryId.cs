using HealthUrWelath.Application.Common.Caching;
using MediatR;

public static class GetProductsByCategoryId
{
    public sealed record Query(int CategoryId)
        : IRequest<IReadOnlyList<ProductListDto>>;

    public sealed class Handler
        : IRequestHandler<Query, IReadOnlyList<ProductListDto>>
    {
        private readonly IProductReadRepository _repo;
        private readonly IAppCache _cache;

        public Handler(
            IProductReadRepository repo,
            IAppCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public Task<IReadOnlyList<ProductListDto>> Handle(
            Query q,
            CancellationToken ct)
        {
            var cacheKey = $"products:category:{q.CategoryId}";

            return _cache.GetOrCreateAsync(
                cacheKey,
                TimeSpan.FromMinutes(10),
                () => _repo.GetProductsByCategoryIdAsync(q.CategoryId));
        }
    }
}