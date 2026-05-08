using HealthUrWelath.Application.Home.Dtos;

namespace HealthUrWelath.Application.Home.Interfaces
{
    public interface INavigationReadRepository
    {
        Task<IReadOnlyList<NavigationDto>> GetNavigationAsync();
        Task<IReadOnlyList<BrandGroupDto>> GetBrandsAsync();
        Task<IReadOnlyList<BrandGroupDto>> GetBrandsBySubCategoryAsync(int subCategoryId);
        Task<IReadOnlyList<BrandGroupDto>> GetBrandsByCategoryAsync(int categoryId);
    }
}
