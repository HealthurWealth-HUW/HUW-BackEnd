using HealthUrWelath.Application.Addresses.Dtos;

namespace HealthUrWelath.Application.Addresses.Interfaces
{
    public interface IAddressRepository
    {
        Task<IReadOnlyList<UserAddressDto>> GetByUserAsync(long userId, long? userAddressId);
        Task<long> AddAsync(AddAddressDto   dto, long userId);
        Task UpdateAsync(UpdateAddressDto dto, long userId);
        Task DeleteAsync(long addressId, long userId);
    }
}
