using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Dtos;
using HealthUrWelath.Application.Cart.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Cart.Commands
{
    public static class ApplyCoupon
    {
        public sealed record Command(string CouponCode) : IRequest<CouponResultDto>;

        public sealed class Handler : IRequestHandler<Command, CouponResultDto>
        {
            private readonly ICartRepository _repo;
            private readonly IUserContext _userContext;

            public Handler(ICartRepository repo, IUserContext userContext)
            {
                _repo = repo;
                _userContext = userContext;
            }

            public async Task<CouponResultDto> Handle(Command q, CancellationToken ct)
            {
                // Get or create cart id via repository
                var cartId = await _repo.GetOrCreateCartAsync(_userContext.UserId, null);

                // Call stored proc to apply coupon
                var result = await _repo.ApplyCouponAsync(cartId, _userContext.UserId, q.CouponCode);

                if (result == null)
                {
                    return new CouponResultDto { Success = false, Message = "Failed to apply coupon" };
                }

                return new CouponResultDto
                {
                    Success = true,
                    Message = "Coupon applied",
                    DiscountAmount = result.DiscountAmount,
                    CouponCode = q.CouponCode,
                    MinCartValue = 0,
                    CouponAmount = result.DiscountAmount,
                    CouponPercentage = null
                };
            }
        }
    }
}
