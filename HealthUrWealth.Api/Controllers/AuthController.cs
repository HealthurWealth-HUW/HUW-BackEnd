using HealthUrWealth.Api.Contracts.Auth;
using HealthUrWelath.Application.Authentication.Commands;
using HealthUrWelath.Application.Authentication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpPost("request-otp")]
        public Task<OtpResultDto> RequestOtp(
    [FromBody] RequestOtpRequest req)
        {
            return _mediator.Send(
                new RequestOtp.Command(req.Mobile));
        }

        [HttpPost("verify-otp")]
        public Task<AuthTokenDto> VerifyOtp(
    [FromBody] VerifyOtpRequest request)
        {
            return _mediator.Send(
                new VerifyOtp.Command(
                    request.Mobile,
                    request.Otp,
                    request.GuestId
                ));
        }
    }
}
