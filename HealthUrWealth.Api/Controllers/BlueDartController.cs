using HealthUrWelath.Application.BlueDart.Commands;
using HealthUrWelath.Application.Home.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlueDartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlueDartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("get-edd")]
        public async Task<IActionResult> GetEDD([FromBody] PincodeRequest request)
        {
            var result = await _mediator.Send(new GetEDD.Command(request.Pincode));

            return Ok(result);
        }
    }
}
