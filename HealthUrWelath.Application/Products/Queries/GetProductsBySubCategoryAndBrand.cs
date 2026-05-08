using HealthUrWelath.Application.Common.Caching;
using MediatR;

public static class GetProductsBySubCategoryAndBrand
{
    public sealed record Query(int SubCategoryId, string Brand) : IRequest<IReadOnlyList<ProductListDto>>;

    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<ProductListDto>>
    {
        private readonly IProductReadRepository _repo;
        private readonly IAppCache _cache;

        public Handler(IProductReadRepository repo, IAppCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public Task<IReadOnlyList<ProductListDto>> Handle(Query q, CancellationToken ct)
        {
            var cacheKey = $"products:subCategory:{q.SubCategoryId}:brand:{q.Brand}";

            return _cache.GetOrCreateAsync(
                cacheKey,
                TimeSpan.FromMinutes(10),
                () => _repo.GetProductsBySubCategoryAndBrandAsync(q.SubCategoryId, q.Brand));
        }
    }
}
