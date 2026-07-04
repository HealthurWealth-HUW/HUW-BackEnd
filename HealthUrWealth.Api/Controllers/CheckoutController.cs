using HealthUrWelath.Application.Checkout.Commands;
using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Checkout.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public sealed class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;

    public CheckoutController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartCheckoutDto checkoutDto)
    {
        var result = await _mediator.Send(new StartCheckout.Command(checkoutDto));
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<CheckoutSummaryDto>> Summary()
        => Ok(await _mediator.Send(new GetCheckoutSummary.Query()));

    [HttpGet("items")]
    public async Task<IActionResult> Items()
        => Ok(await _mediator.Send(new GetCheckoutItems.Query()));

    [HttpPost("confirm-cod")]
    public async Task<IActionResult> ConfirmCod([FromBody] ConfirmCheckoutRequestDto request)
        => Ok(await _mediator.Send(new ConfirmCheckoutCOD.Command(request.paymentTransactionId)));
}
