using HealthUrWelath.Application.Users.Commands;
using HealthUrWelath.Application.Users.Dtos;
using HealthUrWelath.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    [Authorize]
    public sealed class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var result = await _mediator.Send(new GetUserProfile.Query());
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UserProfileDto dto)
        {
            await _mediator.Send(
                new UpdateUserProfile.Command(
                    dto.FirstName,
                    dto.LastName,
                    dto.EmailId,
                   // dto.MobileNo,
                    dto.AlternateMobileNo
                )
            );

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}
