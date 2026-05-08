using HealthUrWelath.Application.Home.Dtos;
using HealthUrWelath.Application.Products.Dto;

public interface IProductReadRepository
{
    Task<IReadOnlyList<ProductSummaryDto>> GetFeaturedProductsAsync(int topN);
    Task<IReadOnlyList<ProductSummaryDto>> GetLatestProductsAsync(int topN);
    Task<IReadOnlyList<ProductSummaryDto>> GetCaronaProductsAsync(int topN);
    Task<IReadOnlyList<ProductSummaryDto>> GetBestSoldProductsAsync(int topN, int minSalesCount);
    Task<ProductDetailsDto?> GetProductByIdAsync(long productId);
    Task<ProductAdditionalInfoDto?> GetProductAdditionalInfoAsync(long productId);
    Task<IReadOnlyList<ProductListDto>> GetProductsByCategoryIdAsync(int categoryId);
    Task<IReadOnlyList<ProductListDto>> GetProductsBySubCategoryIdAsync(int subCategoryId);
    Task<IReadOnlyList<ProductListDto>> GetProductsByBrandAsync(string brand);
    Task<IReadOnlyList<ProductListDto>> GetProductsByCategoryAndBrandAsync(int categoryId, string brand);
    Task<IReadOnlyList<ProductListDto>> GetProductsBySubCategoryAndBrandAsync(int subCategoryId, string brand);
    Task<IReadOnlyList<SubProductDto>> GetSubProductsAsync(long productId);
    Task<IReadOnlyList<ProductSummaryDto>> GetRelatedProductsAsync(long productId);
    Task<IReadOnlyList<ProductSearchDto>> SearchProductsAsync(string searchText);
    Task<long> NotifyMeInsertAsync( long productId,string name,  long mobile,  string email);
}
