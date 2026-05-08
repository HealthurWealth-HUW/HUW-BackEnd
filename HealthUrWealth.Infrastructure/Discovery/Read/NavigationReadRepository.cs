using Dapper;
using HealthUrWealth.Infrastructure.Discovery.Read.Models;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Home.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Discovery.Read;

public sealed class NavigationReadRepository : INavigationReadRepository
{
    private readonly IDbConnection _db;

    public NavigationReadRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<NavigationDto>> GetNavigationAsync()
    {
        var rows = await _db.QueryAsync<NavigationFlatRow>(
            "SP_Discovery_Navigation",
            commandType: CommandType.StoredProcedure);

        return rows
            .GroupBy(x => new { x.SuperCategoryId, x.SuperCategoryName })
            .Select(sc => new NavigationDto
            {
                SuperCategoryId = sc.Key.SuperCategoryId,
                SuperCategoryName = sc.Key.SuperCategoryName,
                Categories = sc
                    .GroupBy(c => new { c.CategoryId, c.CategoryName })
                    .Select(cat => new NavigationCategoryDto
                    {
                        CategoryId = cat.Key.CategoryId,
                        CategoryName = cat.Key.CategoryName,
                        SubCategories = cat
                            .Select(s => new NavigationSubCategoryDto
                            {
                                SubCategoryId = s.SubCategoryId,
                                SubCategoryName = s.SubCategoryName
                            })
                            .DistinctBy(x => x.SubCategoryId)
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();
    }
    public async Task<IReadOnlyList<BrandGroupDto>> GetBrandsAsync()
    {
        var rows = await _db.QueryAsync<BrandGroupDto>(
            "SP_GetBrandsWithCount",
            commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<IReadOnlyList<BrandGroupDto>> GetBrandsBySubCategoryAsync(int subCategoryId)
    {
        var rows = await _db.QueryAsync<BrandGroupDto>(
            "SP_GetBrandsBySubCategory",
            new { SubCategoryId = subCategoryId },
            commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<IReadOnlyList<BrandGroupDto>> GetBrandsByCategoryAsync(int categoryId)
    {
        var rows = await _db.QueryAsync<BrandGroupDto>(
            "SP_GetBrandsByCategory",
            new { CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }
}
