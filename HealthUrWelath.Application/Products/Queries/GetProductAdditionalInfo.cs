using MediatR;

public static class GetProductAdditionalInfo
{
    public sealed record Query(long ProductId)
        : IRequest<ProductAdditionalInfoDto?>;

    public sealed class Handler
        : IRequestHandler<Query, ProductAdditionalInfoDto?>
    {
        private readonly IProductReadRepository _repo;

        public Handler(IProductReadRepository repo)
        {
            _repo = repo;
        }

        public Task<ProductAdditionalInfoDto?> Handle(Query q, CancellationToken ct)
            => _repo.GetProductAdditionalInfoAsync(q.ProductId);
    }
}
