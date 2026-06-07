using HealthUrWelath.Application.Common.Caching;
using MediatR;

public static class GetProductById
{
    public sealed record Query(long ProductId)
        : IRequest<ProductDetailsDto?>;

    public sealed class Handler
        : IRequestHandler<Query, ProductDetailsDto?>
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

        public Task<ProductDetailsDto?> Handle(
            Query request,
            CancellationToken ct)
        {
            return _cache.GetOrCreateAsync(
                featureName: "Products",
                key: $"product:{request.ProductId}:v1",
                factory: () => _repository.GetProductByIdAsync(request.ProductId));
        }
    }
}
