using HealthUrWelath.Application.Addresses.Dtos;
using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.BlueDart.Interfaces;
using HealthUrWelath.Application.Checkout.Dtos;
using HealthUrWelath.Application.Checkout.Interfaces;
using HealthUrWelath.Application.Common.Exceptions;
using HealthUrWelath.Application.Common.Interfaces;
using MediatR;

namespace HealthUrWelath.Application.Checkout.Commands
{
    public static class StartCheckout
    {
        public sealed record Command(
    StartCheckoutDto checkoutDto
            ) : IRequest<CheckoutResultDto>;

        public sealed class Handler
    : IRequestHandler<Command, CheckoutResultDto>
        {
            private readonly ICheckoutRepository _repo;
            private readonly IUserContext _user;
            private readonly ICurrencyContext _currency;
            private readonly IBluedartService _blueDartsvc;
            private readonly IAddressRepository _addressRepository;

            public Handler(
                ICheckoutRepository repo,
                IUserContext user, ICurrencyContext currency, IBluedartService blueDartsvc, IAddressRepository addressRepository)
            {
                _repo = repo;
                _user = user;
                _currency = currency;
                _blueDartsvc = blueDartsvc;
                _addressRepository = addressRepository;
            }

            public async Task<CheckoutResultDto> Handle(
                Command request,
                CancellationToken ct)
            {
                var addresses = await _addressRepository.GetByUserAsync(_user.UserId, request.checkoutDto.ShippingAddressId);
                var address = addresses.FirstOrDefault()
                    ?? throw new AppException("Shipping address not found.", 422);

                var eddData = await _blueDartsvc.GetEDD(address.PinCode);

                // Ensure we never pass an uninitialized DateTime (SQL DateTime minimum is 1753)
                var expectedDelivery = eddData.DeliveryDate
                    ?? eddData.DeliveryDateWhenNoEDD
                    ?? DateTime.UtcNow.AddDays(4);

                var txnId = await _repo.UpsertAndCalculateAsync(
                    _user.UserId,
                    request.checkoutDto.PaymentMode,

                    request.checkoutDto.BillingAddressId,
                    request.checkoutDto.ShippingAddressId,
                    expectedDelivery,

                    _currency.CurrencyCode,
                    _currency.CurrencySymbol,
                    _currency.CurrencyValue);
                return new CheckoutResultDto(txnId);
            }
        }

    }
}
