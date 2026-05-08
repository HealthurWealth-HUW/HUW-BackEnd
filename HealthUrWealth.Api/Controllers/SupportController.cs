using HealthUrWealth.Api.Contracts.Impersonation;
using HealthUrWealth.Application.Impersonate.Commands;
using HealthUrWealth.Application.Impersonate.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Support")]
    public sealed class SupportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("impersonate/start")]
        public async Task<IActionResult> Start(
    StartImpersonationRequest req)
        {
            var supportUserId =
                long.Parse(User.FindFirst("sub")!.Value);

            var ip =
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "NA";

            var token = await _mediator.Send(
                new StartImpersonation.Command(
                    supportUserId,
                    req.CustomerUserId,
                    req.Reason,
                    ip));

            return Ok(token);
        }

        [HttpPost("impersonate/end")]
        public async Task<IActionResult> End(
            [FromBody] long customerUserId)
        {
            var supportUserId =
                long.Parse(User.FindFirst("sub")!.Value);

            await _mediator.Send(
                new EndImpersonation.Command(
                    supportUserId,
                    customerUserId));

            return Ok();
        }

    }
}
