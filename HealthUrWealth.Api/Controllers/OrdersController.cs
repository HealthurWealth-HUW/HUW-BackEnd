using HealthUrWealth.Application.Orders.Queries;
using HealthUrWelath.Application.Orders.Dtos;
using HealthUrWelath.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] 
    public sealed class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _mediator.Send(new GetOrders.Query());

            var OrderDetails = orders.GroupBy(o => o.OrderId).Select(g => new OrdersDto
            {
                OrderId = g.Key,
                UserId = g.First().UserId,
                TxnAmount = g.First().TxnAmount,
                PaymentMode = g.First().PaymentMode,
                PaymentStatus = g.First().PaymentStatus,
                OrderCurrentStatus = g.First().OrderCurrentStatus,
                ShippingCharges = g.First().ShippingCharges,
                CouponDiscount = g.First().CouponDiscount,
                IsCartTampered = g.First().IsCartTampered,
                OrderDate = g.First().OrderDate,
                ProductDetails = g.Select(x => new ProductDetails
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductImgUrl = x.ProductImgUrl,
                    Quantity = x.Quantity,
                    ActualAmount = x.ActualAmount,
                    ItemCouponDiscount = x.ItemCouponDiscount,
                    FinalAmount = x.FinalAmount,
                    ExpectedDeliveryDate = x.ExpectedDeliveryDate
                }).ToList()
            }).ToList(); return Ok(OrderDetails);
        }
        [HttpGet("orderdetails/{orderId:int}")]
        public async Task<IActionResult> GetOrdersDetails(int orderId)
        {
            var orderDetails = await _mediator.Send(new GetOrderDetails.Query(orderId));
            return Ok(orderDetails);
        }

        [HttpGet("{orderId}/status")]
        public async Task<IActionResult> GetOrderStatus(long orderId)
        {
            var result = await _mediator.Send(
                new GetOrderTimeline.Query(orderId));

            return Ok(result);
        }
    }
}