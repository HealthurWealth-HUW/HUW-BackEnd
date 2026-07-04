using Dapper;
using HealthUrWelath.Application.Authentication.Dtos;
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

        public async Task<OtpValidationResult> ValidateAsync(
            string mobile,
            string otp,
            int maxAttempts,
            int lockoutMinutes)
        {
            return await _db.QueryFirstAsync<OtpValidationResult>(
                "SP_UserOtp_Validate",
                new
                {
                    Mobile = mobile,
                    OtpCode = otp,
                    MaxAttempts = maxAttempts,
                    LockoutMinutes = lockoutMinutes
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
