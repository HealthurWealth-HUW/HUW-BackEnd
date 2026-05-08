using Dapper;
using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Users.Dtos;
using HealthUrWelath.Application.Users.Interfaces;
using System.Data;
using System.Data.Common;

namespace HealthUrWealth.Infrastructure.Users
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<UserDto> GetOrCreateByMobileAsync(string mobile, string? email)
        {
            var rows = await _db.QueryAsync<UserDto>(
                "SP_User_GetOrCreateByMobile",
                new { MobileNo = mobile, EmailId = email },
                commandType: CommandType.StoredProcedure);
            return rows.Single();
        }


        public Task<long?> GetUserIdByMobileAsync(string mobile)
            => _db.ExecuteScalarAsync<long?>(
                "SELECT UserId FROM Users WHERE MobileNo = @mobile",
                new { mobile });

        public Task<bool> IsSupportAgentAsync(long userId)
            => _db.ExecuteScalarAsync<bool>(
                "SELECT CASE WHEN RoleId IN (3,4) THEN 1 ELSE 0 END FROM Users WHERE UserId = @userId",
                new { userId });

        public async Task<UserProfileDto> GetByIdAsync(long userId)
        {
            return await _db.QueryFirstOrDefaultAsync<UserProfileDto>(
                "SP_User_GetById",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task UpdateProfileAsync(
            long userId,
            string firstName,
            string lastName,
            string emailId,
           // string mobileNo,
            string alternateMobileNo,
            CancellationToken cancellationToken = default)
        {
            await _db.ExecuteAsync(
                "SP_User_UpdateProfile",
                new
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailId = emailId,
                   // MobileNo = mobileNo,
                    AlternateContactNumber = alternateMobileNo,
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
