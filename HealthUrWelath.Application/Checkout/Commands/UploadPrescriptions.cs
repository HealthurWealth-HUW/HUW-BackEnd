using HealthUrWealth.Api.Contracts.Cart;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Cart.Interfaces;
using HealthUrWelath.Application.Common.CDN;
using HealthUrWelath.Application.Common.Enums;
using MediatR;
namespace HealthUrWelath.Application.Checkout.Commands
{
    public class UploadPrescriptions
    {
        public sealed record Command(UploadPrescriptionDto Prescription)
            : IRequest<int>;

        public sealed class Handler
            : IRequestHandler<Command, int>
        {
            private readonly ICartRepository _repo;
            private readonly IBunnyCdnStorageService _storage;
            private readonly IUserContext _user;

            public Handler(
                ICartRepository repo,
                IBunnyCdnStorageService storage,
                IUserContext user)
            {
                _repo = repo;
                _storage = storage;
                _user = user;
            }

            public async Task<int> Handle(
                Command request,
                CancellationToken ct)
            {
                var urls = await _storage.UploadAsync(request.Prescription.Files, CdnUploadTarget.Prescription);

                return await _repo.InsertPrescriptionsAsync(
                    _user.UserId,
                    request.Prescription.CartId,
                    urls.ToList());
            }
        }
    }
}
