using HealthUrWelath.Application.Home.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class DiscoveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscoveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHome()
        {
            var result = await _mediator.Send(new GetProductsByCategoryGroup.Query());
            return Ok(result);
        }
    }
}
