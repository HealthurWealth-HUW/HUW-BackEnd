using Dapper;
using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Addresses
{
    public sealed class AddressRepository : IAddressRepository
    {
        private readonly IDbConnection _db;

        public AddressRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<UserAddressDto>> GetByUserAsync(long userId, long? userAddressId)
        {

            var result = await _db.QueryAsync<UserAddressDto>(
        "SP_UserAddress_GetByUser",
        new
        {
            UserId = userId,
            UserAddressId = userAddressId
        },
        commandType: CommandType.StoredProcedure
    );

            return result.ToList();
        }

        public async Task<long> AddAsync(AddAddressDto dto, long userId)
        {
            return await _db.ExecuteScalarAsync<long>(
                "SP_UserAddress_Add",
                new
                {
                    UserId = userId,
                    dto.AddressTypeId,
                    dto.CountryId,
                    dto.StateId,
                    dto.StateName,
                    dto.City,
                    dto.StreetAddress1,
                    dto.StreetAddress2,
                    dto.LandMark,
                    dto.PinCode
                },
                commandType: CommandType.StoredProcedure);
        }

        public Task UpdateAsync(UpdateAddressDto dto, long userId)
        {
            return _db.ExecuteAsync(
                "SP_UserAddress_Update",
                new
                {
                    dto.UserAddressId,
                    UserId = userId,
                    dto.AddressTypeId,
                    dto.CountryId,
                    dto.StateId,
                    dto.StateName,
                    dto.City,
                    dto.StreetAddress1,
                    dto.StreetAddress2,
                    dto.LandMark,
                    dto.PinCode
                },
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteAsync(long addressId, long userId)
        {
            return _db.ExecuteAsync(
                "SP_UserAddress_Delete",
                new
                {
                    UserAddressId = addressId,
                    UserId = userId
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
