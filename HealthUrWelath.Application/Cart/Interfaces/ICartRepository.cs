using HealthUrWelath.Application.Cart.Dtos;

namespace HealthUrWelath.Application.Cart.Interfaces
{
    public interface ICartRepository
    {
        Task<long> GetOrCreateCartAsync(long? userId, Guid? guestId);

        Task<IReadOnlyList<CartItemDto>> GetCartItemsAsync(long cartId);

        Task AddOrUpdateItemAsync(long cartId, long productId, int quantity);
        Task UpdateQuantityAsync(long userId, long productId, int quantity);

        Task RemoveItemAsync(long cartId, long productId);

        Task MergeGuestCartAsync(Guid guestId, long userId);
        Task<CouponInfo?> GetCouponByCodeAsync(string couponCode, bool includeExpired = false);

        Task<ApplyCouponResultDto> ApplyCouponAsync(long cartId, long? userId, string couponCode);

        Task<int> InsertPrescriptionsAsync(long userId, long CartId, List<string> imageUrls);
    }
}
