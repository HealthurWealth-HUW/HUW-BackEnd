using HealthUrWelath.Application.Addresses.Commands;
using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public sealed class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<IReadOnlyList<UserAddressDto>> Get()
            => _mediator.Send(new GetUserAddresses.Query(null));

        [HttpGet("{userAddressId:long}")]
        public Task<IReadOnlyList<UserAddressDto>> GetById(long userAddressId)
           => _mediator.Send(new GetUserAddresses.Query(userAddressId));

        [HttpPost]
        public async Task<IActionResult> Add(AddAddressDto dto)
        {
            await _mediator.Send(new AddAddress.Command(dto));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAddressDto dto)
        {
            await _mediator.Send(new UpdateAddress.Command(dto));
            return Ok();
        }

        [HttpDelete("{addressId:long}")]
        public async Task<IActionResult> Delete(long addressId)
        {
            await _mediator.Send(new DeleteAddress.Command(addressId));
            return Ok();
        }
    }
}
