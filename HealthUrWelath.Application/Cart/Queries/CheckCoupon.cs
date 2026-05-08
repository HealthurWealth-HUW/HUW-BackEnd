using HealthUrWelath.Application.Cart.Dtos;
using HealthUrWelath.Application.Cart.Interfaces;
using HealthUrWelath.Application.Common.Caching;
using MediatR;

namespace HealthUrWelath.Application.Cart.Queries
{
    public static class CheckCoupon
    {
        public sealed record Query(string CouponCode, long CartId, decimal CartTotal) : IRequest<CouponResultDto>;

        public sealed class Handler : IRequestHandler<Query, CouponResultDto>
        {
            private readonly ICartRepository _repo;
            private readonly IAppCache _cache;

            public Handler(ICartRepository repo, IAppCache cache)
            {
                _repo = repo;
                _cache = cache;
            }

            public async Task<CouponResultDto> Handle(Query q, CancellationToken ct)
            {
                var coupon = await _repo.GetCouponByCodeAsync(q.CouponCode);

                if (coupon == null || !coupon.Status || coupon.Valid_From >= DateTime.Now)
                {
                    return new CouponResultDto { Success = false, Message = "Invalid Coupon Code" };
                }

                if (coupon.Valid_To.HasValue && coupon.Valid_To.Value < DateTime.Now)
                {
                    return new CouponResultDto { Success = false, Message = "Coupon expired... Try with another coupon" };
                }

                var minCart = coupon.Min_Cart_Value ?? 0m;
                if (q.CartTotal < minCart)
                {
                    return new CouponResultDto { Success = false, Message = $"Minimum Order Amount for this coupon is: {minCart}", MinCartValue = minCart };
                }

                decimal couponAmount = 0m;
                if (coupon.Coupon_Percentage.HasValue && coupon.Coupon_Percentage.Value > 0)
                {
                    couponAmount = Math.Round((q.CartTotal * coupon.Coupon_Percentage.Value) / 100m, 0);
                }
                else if (coupon.Coupon_Amount.HasValue)
                {
                    couponAmount = coupon.Coupon_Amount.Value;
                }

                return new CouponResultDto
                {
                    Success = true,
                    Message = "Coupon code applied successfully",
                    DiscountAmount = couponAmount,
                    CouponCode = coupon.Coupon_Code,
                    MinCartValue = minCart,
                    CouponAmount = coupon.Coupon_Amount ?? 0m,
                    CouponPercentage = coupon.Coupon_Percentage
                };
            }
        }
    }
}
