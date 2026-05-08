using HealthUrWelath.Application.Home.Interfaces;
using System.Data;
using Dapper;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWealth.Infrastructure.Discovery.Read.Models;
using HealthUrWelath.Application.Checkout.Dtos;

namespace HealthUrWealth.Infrastructure.Discovery.Read
{
    public sealed class DiscoveryReadRepository : IDiscoveryReadRepository
    {
        private readonly IDbConnection _db;

        public DiscoveryReadRepository(IDbConnection db)
        {
            _db = db;
        }
        
        public async Task<IReadOnlyList<CategoryGroupDto>>
            GetCategoryGroupsAsync(int topProductsPerCategory)
        {
            var rows = await _db.QueryAsync<DiscoveryProductFlatRow>(
                "SP_Discovery_ProductsByCategoryGroup",
                new { TopProductsPerCategory = topProductsPerCategory },
                commandType: CommandType.StoredProcedure);

            return rows
                .GroupBy(x => new
                {
                    x.SuperCategoryId,
                    x.SuperCategoryName,
                    x.CategoryId,
                    x.CategoryName
                })
                .Select(g => new CategoryGroupDto
                {
                    SuperCategoryId = g.Key.SuperCategoryId,
                    SuperCategoryName = g.Key.SuperCategoryName,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Products = g.Select(p => new ProductSummaryDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductCost = p.ProductCost,
                        ProductOriginalCost = p.ProductOriginalCost,
                        ProductDiscountPercentage = p.ProductDiscountPercentage,
                        ProductDiscountCost = p.ProductDiscountCost,
                        ProductImgUrl = p.ProductImgUrl,
                        ProductImgUrl2 = p.ProductImgUrl2
                    }).ToList()
                })
                .ToList();
        }      
    }

}
