using HealthUrWelath.Application.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace HealthUrWelath.Application.Common.CDN
{
    public interface IBunnyCdnStorageService
    {
        Task<IReadOnlyList<string>> UploadAsync(IEnumerable<IFormFile> files, CdnUploadTarget target = CdnUploadTarget.None);

        Task<string> UploadAsync(Stream stream, string fileName, CdnUploadTarget target = CdnUploadTarget.None);
    }
}
