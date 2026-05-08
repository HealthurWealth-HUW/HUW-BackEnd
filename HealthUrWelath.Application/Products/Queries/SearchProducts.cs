using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Products.Dto;
using MediatR;

namespace HealthUrWelath.Application.Products.Queries
{
    public class SearchProducts
    {
        public sealed record Query(string SearchText)
            : IRequest<IReadOnlyList<ProductSearchDto>>;

        public sealed class Handler
            : IRequestHandler<Query, IReadOnlyList<ProductSearchDto>>
        {
            private readonly IProductReadRepository _repo;

            public Handler(IProductReadRepository repo)
            {
                _repo = repo;
            }

            public async Task<IReadOnlyList<ProductSearchDto>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                return await _repo.SearchProductsAsync(request.SearchText);
            }
        }
    }
}
