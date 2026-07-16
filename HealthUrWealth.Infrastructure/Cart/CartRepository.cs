using Dapper;
using HealthUrWelath.Application.Cart.Dtos;
using HealthUrWelath.Application.Cart.Interfaces;
using System.Data;

namespace HealthUrWealth.Infrastructure.Cart
{
    public sealed class CartRepository : ICartRepository
    {
        private readonly IDbConnection _db;

        public CartRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<long> GetOrCreateCartAsync(long? userId, Guid? guestId)
        {
            return await _db.ExecuteScalarAsync<long>(
                "SP_Cart_GetOrCreate",
                new { UserId = userId, GuestId = guestId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IReadOnlyList<CartItemDto>> GetCartItemsAsync(long cartId)
        {
            var data = await _db.QueryAsync<CartItemDto>(
                "SP_Cart_GetItems",
                new { CartId = cartId },
                commandType: CommandType.StoredProcedure);

            return data.ToList();
        }

        public Task AddOrUpdateItemAsync(long cartId, long productId, int quantity)
            => _db.ExecuteAsync(
                "SP_Cart_AddOrUpdateItem",
                new { CartId = cartId, ProductId = productId, Quantity = quantity },
                commandType: CommandType.StoredProcedure);

        public async Task UpdateQuantityAsync(long userId, long productId, int quantity)
        {
            await _db.ExecuteAsync(
                "SP_Cart_UpdateQuantity",
                new
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                },
                commandType: CommandType.StoredProcedure);
        }

        public Task RemoveItemAsync(long cartId, long productId)
            => _db.ExecuteAsync(
                "SP_Cart_RemoveItem",
                new { CartId = cartId, ProductId = productId },
                commandType: CommandType.StoredProcedure);

        public Task MergeGuestCartAsync(Guid guestId, long userId)
            => _db.ExecuteAsync(
                "SP_Cart_MergeGuestToUser",
                new { GuestId = guestId, UserId = userId },
                commandType: CommandType.StoredProcedure);
        public async Task<CouponInfo?> GetCouponByCodeAsync(string couponCode, bool includeExpired = false)
        {
            var result = await _db.QueryFirstOrDefaultAsync<CouponInfo>(
                "SP_GetCouponByCode",
                new { CouponCode = couponCode },
                commandType: CommandType.StoredProcedure);

            if (result == null)
                return null;

            if (!includeExpired)
            {
                if (!result.Status)
                    return null;

                if (result.Valid_From.HasValue && result.Valid_From.Value > DateTime.Now)
                    return null;
            }

            return result;
        }
        public async Task<int> InsertPrescriptionsAsync(long userId, long CartId, List<string> imageUrls)
        {
            var table = new DataTable();
            table.Columns.Add("ImageUrl", typeof(string));

            foreach (var url in imageUrls)
                table.Rows.Add(url);

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@CartId", CartId);
            parameters.Add("@Images", table.AsTableValuedParameter("PrescriptionImageList"));

            return await _db.ExecuteScalarAsync<int>(
                "SP_Prescription_InsertMultiple",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<ApplyCouponResultDto?> ApplyCouponAsync(long cartId, long? userId, string couponCode)
        {
            var result = await _db.QueryFirstOrDefaultAsync<ApplyCouponResultDto>(
                "SP_Cart_ApplyCoupon",
                new { CartId = cartId, UserId = userId, CouponCode = couponCode },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public Task RemoveCouponAsync(long cartId, long userId)
            => _db.ExecuteAsync(
                "SP_Cart_RemoveCoupon",
                new { CartId = cartId, UserId = userId },
                commandType: CommandType.StoredProcedure);
    }
}
