using HealthUrWelath.Application.Home.Queries.Products;
using HealthUrWelath.Application.Products.Commands;
using HealthUrWelath.Application.Products.Dto;
using HealthUrWelath.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            return Ok(await _mediator.Send(new GetFeaturedProducts.Query()));
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            return Ok(await _mediator.Send(new GetLatestProducts.Query()));
        }

        [HttpGet("best-sold")]
        public async Task<IActionResult> GetBestSold()
        {
            return Ok(await _mediator.Send(new GetBestSoldProducts.Query()));
        }

        [HttpGet("carona")]
        public async Task<IActionResult> GetCarona()
        {
            return Ok(await _mediator.Send(new GetCaronaProducts.Query()));
        }

        [HttpGet("by-category/{categoryId:int}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            return Ok(await _mediator.Send(
                new GetProductsByCategoryId.Query(categoryId)));
        }

        [HttpGet("by-subcategory/{subCategoryId:int}")]
        public async Task<IActionResult> GetBySubCategory(int subCategoryId)
        {
            return Ok(await _mediator.Send(
                new GetProductsBySubCategoryId.Query(subCategoryId)));
        }

        [HttpGet("by-brand/{brand}")]
        public async Task<IActionResult> GetByBrand(string brand)
        {
            return Ok(await _mediator.Send(
                new GetProductsByBrand.Query(brand)));
        }

        [HttpGet("by-category/{categoryId:int}/brand/{brand}")]
        public async Task<IActionResult> GetByCategoryAndBrand(int categoryId, string brand)
        {
            var result = await _mediator.Send(new GetProductsByCategoryAndBrand.Query(categoryId, brand));
            return Ok(result);
        }

        [HttpGet("by-subcategory/{subCategoryId:int}/brand/{brand}")]
        public async Task<IActionResult> GetBySubCategoryAndBrand(int subCategoryId, string brand)
        {
            var result = await _mediator.Send(new GetProductsBySubCategoryAndBrand.Query(subCategoryId, brand));
            return Ok(result);
        }


        [HttpGet("{productId:long}")]
        public async Task<IActionResult> GetById(long productId)
        {
            var result = await _mediator.Send(
                new GetProductById.Query(productId));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id:long}/additional-info")]
        public async Task<IActionResult> GetAdditionalInfo(long id)
        {
            return Ok(await _mediator.Send(new GetProductAdditionalInfo.Query(id)));
        }

        [HttpGet("{productId}/subproducts")]
        public async Task<IActionResult> GetSubProducts(long productId)
        {
            return Ok(await _mediator.Send(new GetSubProducts.Query(productId)));
        }

        [HttpGet("{productId}/relatedproducts")]
        public async Task<IActionResult> GetRelatedProducts(long productId)
        {
            return Ok(await _mediator.Send(new GetRelatedProducts.Query(productId)));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            return Ok(await _mediator.Send(new SearchProducts.Query(q)));
        }

        [HttpPost("notify-me")]
        public async Task<IActionResult> NotifyMe(
    [FromBody] NotifyMeRequestDto request)
        {
            await _mediator.Send(
                new NotifyMe.Command(
                    request.ProductId,
                    request.Name,
                    request.MobileNumber,
                    request.Email));

            return Ok("You will be notified when the product is available.");
        }
    }
}
