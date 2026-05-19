using HealthUrWealth.Api.Contracts.Cart;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Commands;
using HealthUrWelath.Application.Cart.Dtos;
using HealthUrWelath.Application.Cart.Queries;
using HealthUrWelath.Application.Checkout.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        IUserContext _userContext;
        public CartController(IMediator mediator, IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [HttpGet]
        public Task<CartDto> Get([FromQuery] Guid? guestId)
        {
            var (userId, resolvedGuestId) = ResolveCartOwnerOrThrow(guestId);

            return _mediator.Send(
                new GetCart.Query(userId, resolvedGuestId)
            );
        }

        [HttpPost("items")]
        public Task AddItem([FromBody] AddCartItemRequest req)
        {
            var (userId, guestId) = ResolveCartOwnerOrThrow(req.GuestId);

            return _mediator.Send(
                new AddOrUpdateCartItem.Command(
                    userId,
                    guestId,
                    req.ProductId,
                    req.Quantity
                )
            );
        }

        [HttpPut("update-quantity")]
        public Task UpdateQuantity([FromBody] UpdateCartRequest req)
        {
            return _mediator.Send(new UpdateCartQuantity.Command(
                    req.ProductId,
                    req.Quantity
                ));
        }

        [HttpDelete("items/{productId:long}")]
        public Task RemoveItem(long productId, [FromQuery] Guid? guestId)
        {
            var (userId, resolvedGuestId) = ResolveCartOwnerOrThrow(guestId);

            return _mediator.Send(
                new RemoveCartItem.Command(
                    userId,
                    resolvedGuestId,
                    productId
                )
            );
        }

        [Authorize]
        [HttpPost("merge")]
        public Task Merge([FromQuery] Guid guestId)
        {
            if (guestId == Guid.Empty)
                throw new ArgumentException("guestId is required");

            var userId = _userContext.UserId;

            return _mediator.Send(
                new MergeCart.Command(guestId, userId)
            );
        }

        //[HttpGet("check-coupon")]
        //public async Task<IActionResult> CheckCoupon([FromBody] ApplyCouponRequest request)
        //{
        //    var result = await _mediator.Send(new CheckCoupon.Query(request.CouponCode, request.CartId, request.CartTotal));
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        [Authorize]
        [HttpPost("apply-coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            var result = await _mediator.Send(new ApplyCoupon.Command(request.CouponCode));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("remove-coupon")]
        public async Task<IActionResult> RemoveCoupon([FromBody] RemoveCouponRequest request)
        {
            if (request.CartId <= 0)
                return BadRequest("CartId is required");

            await _mediator.Send(new RemoveCoupon.Command(request.CartId));

            return Ok(new { Success = true, Message = "Coupon removed" });
        }

        [HttpPost("uploadprescriptions")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPrescriptions(
    [FromForm] UploadPrescriptionDto request)
        {
            var count = await _mediator.Send(
                new UploadPrescriptions.Command(request));

            return Ok(new
            {
                UploadedFiles = count
            });
        }

        private (long? userId, Guid? guestId) ResolveCartOwnerOrThrow(Guid? guestId)
        {
            if (_userContext.IsAuthenticated == true)
            {
                return (_userContext.UserId, null);
            }

            if (guestId == null || guestId == Guid.Empty)
                throw new ArgumentException("guestId is required for guest cart");

            return (null, guestId);
        }
    }
}
