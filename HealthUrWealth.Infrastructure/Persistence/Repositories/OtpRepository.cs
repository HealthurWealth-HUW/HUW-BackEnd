using Dapper;
using HealthUrWelath.Application.Authentication.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Persistence.Repositories
{
    public sealed class OtpRepository : IOtpRepository
    {
        private readonly IDbConnection _db;

        public OtpRepository(IDbConnection db)
        {
            _db = db;
        }

        public Task SaveAsync(long userId, string otp)
        {
            return _db.ExecuteAsync(
                "SP_UserOtp_Save",
                new
                {
                    UserId = userId,
                    OtpCode = otp
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<long> ValidateAsync(string mobile, string otp)
        {
            return await _db.ExecuteScalarAsync<long>(
         "SP_UserOtp_Validate",
         new { Mobile = mobile, OtpCode = otp },
         commandType: CommandType.StoredProcedure);
        }
    }
}
