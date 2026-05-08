using HealthUrWealth.Application.Home.Queries;
using HealthUrWelath.Application.Home.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NavigationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetNavigation.Query());
            return Ok(result);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _mediator.Send(new GetBrands.Query());
            return Ok(result);
        }

        [HttpGet("brands/by-subcategory/{subCategoryId:int}")]
        public async Task<IActionResult> GetBrandsBySubCategory(int subCategoryId)
        {
            var result = await _mediator.Send(new GetBrandsBySubCategory.Query(subCategoryId));
            return Ok(result);
        }
        [HttpGet("brands/by-category/{categoryId:int}")]
        public async Task<IActionResult> GetBrandsByCategory(int categoryId)
        {
            var result = await _mediator.Send(new GetBrandsByCategory.Query(categoryId));
            return Ok(result);
        }
    }
}
