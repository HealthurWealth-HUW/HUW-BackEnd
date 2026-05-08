using HealthUrWelath.Application.Home.Dtos;

namespace HealthUrWelath.Application.Home.Interfaces
{
    public interface IDiscoveryReadRepository
    {
        Task<IReadOnlyList<CategoryGroupDto>> GetCategoryGroupsAsync(int topProductsPerCategory);
    }
}
