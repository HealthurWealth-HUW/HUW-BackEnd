using HealthUrWealth.Api.Common;
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
        public async Task<ApiResponse<OtpResultDto>> RequestOtp(
    [FromBody] RequestOtpRequest req)
        {
            var result = await _mediator.Send(
                new RequestOtp.Command(req.Mobile));

            return ApiResponse<OtpResultDto>.Ok(
                result,
                "OTP sent successfully.",
                traceId: HttpContext.TraceIdentifier);
        }

        [HttpPost("verify-otp")]
        public async Task<ApiResponse<AuthTokenDto>> VerifyOtp(
    [FromBody] VerifyOtpRequest request)
        {
            var result = await _mediator.Send(
                new VerifyOtp.Command(
                    request.Mobile,
                    request.Otp,
                    request.GuestId
                ));

            return ApiResponse<AuthTokenDto>.Ok(
                result,
                "Login successful.",
                traceId: HttpContext.TraceIdentifier);
        }
    }
}
