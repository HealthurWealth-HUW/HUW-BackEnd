using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class FileUploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileUploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost("prescriptions")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadPrescriptions([FromForm] UploadPrescriptionDto request)
        //{
        //    if (request?.Files == null || request.Files.Count == 0)
        //        return BadRequest("at least one file is required");

        //    var cmd = new UploadPrescriptions.Command(request);
        //    var result = await _mediator.Send(cmd);

        //    return Ok(new { UploadedCount = result });
        //}
    }
}
