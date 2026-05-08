using Microsoft.AspNetCore.Http;

namespace HealthUrWealth.Api.Contracts.Cart
{
    public sealed class UploadPrescriptionDto
    {
        public long CartId { get; init; }
        public List<IFormFile> Files { get; init; } = new();
    }
}
