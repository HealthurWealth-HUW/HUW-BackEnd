using Dapper;
using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Products.Dto;
using System.Data;
using System.Data.Common;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly IDbConnection _db;

    public ProductReadRepository(IDbConnection db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<ProductSummaryDto>>
           GetFeaturedProductsAsync(int topN)
           => await GetCollectionAsync("FEATURED", topN);

    public async Task<IReadOnlyList<ProductSummaryDto>>
        GetLatestProductsAsync(int topN)
        => await GetCollectionAsync("LATEST", topN);

    public async Task<IReadOnlyList<ProductSummaryDto>>
        GetCaronaProductsAsync(int topN)
        => await GetCollectionAsync("CARONA", topN);

    public async Task<IReadOnlyList<ProductSummaryDto>>
        GetBestSoldProductsAsync(int topN, int minSalesCount)
    {
        return (await _db.QueryAsync<ProductSummaryDto>(
            "SP_Product_Collections_Transactional",
            new { CollectionType = "BEST_SOLD", TopN = topN, MinSalesCount = minSalesCount },
            commandType: CommandType.StoredProcedure))
            .ToList();
    }

    public async Task<ProductDetailsDto?> GetProductByIdAsync(long productId)
    {
        return await _db.QueryFirstOrDefaultAsync<ProductDetailsDto>(
            "SP_Product_GetById",
            new { ProductId = productId },
            commandType: CommandType.StoredProcedure);
    }
    public Task<ProductAdditionalInfoDto?> GetProductAdditionalInfoAsync(long productId)
        => _db.QueryFirstOrDefaultAsync<ProductAdditionalInfoDto>(
            "SP_Product_GetAdditionalInfo",
            new { ProductId = productId },
            commandType: CommandType.StoredProcedure);

    public async Task<IReadOnlyList<ProductListDto>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var data = await _db.QueryAsync<ProductListDto>(
            "SP_Product_GetByCategoryId",
            new { CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);

        return data.ToList();

    }
    public async Task<IReadOnlyList<ProductListDto>> GetProductsBySubCategoryIdAsync(int subCategoryId)
    {
        var data = await _db.QueryAsync<ProductListDto>(
            "SP_Product_GetBySubCategoryId",
            new { SubCategoryId = subCategoryId },
            commandType: CommandType.StoredProcedure);

        return data.ToList();

    }
    public async Task<IReadOnlyList<ProductListDto>> GetProductsByBrandAsync(string brandName)
    {
        var data = await _db.QueryAsync<ProductListDto>(
            "SP_Product_GetByBrand",
            new { BrandName = brandName },
            commandType: CommandType.StoredProcedure);

        return data.ToList();

    }
    public async Task<IReadOnlyList<ProductListDto>> GetProductsByCategoryAndBrandAsync(int categoryId, string brand)
    {
        var data = await _db.QueryAsync<ProductListDto>(
            "SP_Product_GetByCategoryAndBrand",
            new { CategoryId = categoryId, BrandName = brand },
            commandType: CommandType.StoredProcedure);

        return data.ToList();
    }

    public async Task<IReadOnlyList<ProductListDto>> GetProductsBySubCategoryAndBrandAsync(int subCategoryId, string brand)
    {
        var data = await _db.QueryAsync<ProductListDto>(
            "SP_Product_GetBySubCategoryAndBrand",
            new { SubCategoryId = subCategoryId, BrandName = brand },
            commandType: CommandType.StoredProcedure);

        return data.ToList();
    }
    public async Task<IReadOnlyList<ProductSummaryDto>> GetRelatedProductsAsync(long productId)
    {
        var result = await _db.QueryAsync<ProductSummaryDto>(
            "SP_Product_GetRelatedProducts",
            new { ProductId = productId },
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }
    public async Task<IReadOnlyList<SubProductDto>> GetSubProductsAsync(long productId)
    {
        var result = await _db.QueryAsync<SubProductDto>(
                "SP_Product_GetSubProducts",
                new { ProductId = productId },
                commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    private async Task<IReadOnlyList<ProductSummaryDto>>
            GetCollectionAsync(string collectionType, int topN)
    {
        return (await _db.QueryAsync<ProductSummaryDto>(
            "SP_Product_Collections_NonTransactional",
            new { CollectionType = collectionType, TopN = topN },
            commandType: CommandType.StoredProcedure))
            .ToList();
    }

    public async Task<IReadOnlyList<ProductSearchDto>> SearchProductsAsync(string searchText)
    {
        var result = await _db.QueryAsync<ProductSearchDto>(
               "SP_Product_Search",
               new { SearchText = searchText },
               commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<long> NotifyMeInsertAsync(long productId, string name, long mobile, string email)
    {
        return await _db.ExecuteScalarAsync<long>(
                "SP_NotifyMe_Insert",
                new
                {
                    ProductId = productId,
                    UserName = name,
                    MobileNumber = mobile,
                    EmailId = email
                },
                commandType: CommandType.StoredProcedure);
    }
}
