using System.Net.Http.Headers;
using HealthUrWelath.Application.Common.Enums;
using HealthUrWelath.Application.Common.CDN;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HealthUrWealth.Infrastructure.Common.CDN
{
    public class BunnyCdnStorageService : IBunnyCdnStorageService
    {
        private readonly HttpClient _http;
        private readonly string _cdnBaseUrl;
        private readonly string _accessKey;
        private readonly Dictionary<CdnUploadTarget, string> _folderMappings = new();
        private readonly string _uploadUrlTemplate;

        public BunnyCdnStorageService(HttpClient http, IConfiguration config)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _cdnBaseUrl = config["CDN:BaseUrl"]?.TrimEnd('/');
            _accessKey = config["CDN:AccessKey"];
            _uploadUrlTemplate = config["CDN:UploadUrl"];

            var foldersSection = config.GetSection("CDN:Folders");
            foreach (var child in foldersSection.GetChildren())
            {
                var key = child.Key;
                var val = child.Value;
                if (string.IsNullOrWhiteSpace(val))
                    continue;

                if (Enum.TryParse<CdnUploadTarget>(key, true, out var parsed))
                {
                    _folderMappings[parsed] = val;
                }
            }

            if (_folderMappings.Count == 0)
                throw new InvalidOperationException("No CDN folder mappings found under CDN:Folders in appsettings. Add entries like CDN:Folders:Prescription = 'prescriptions'.");
        }

        public async Task<IReadOnlyList<string>> UploadAsync(IEnumerable<IFormFile> files, CdnUploadTarget target = CdnUploadTarget.None)
        {
            if (files == null) return Array.Empty<string>();

            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                await using var stream = file.OpenReadStream();
                var url = await UploadAsync(stream, file.FileName, target);
                urls.Add(url);
            }

            return urls;
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, CdnUploadTarget target = CdnUploadTarget.None)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

                if (target == CdnUploadTarget.None || !_folderMappings.TryGetValue(target, out var effectiveFolder))
                    throw new InvalidOperationException("CDN upload target must be specified and mapped.");

                var uploadBase = !string.IsNullOrWhiteSpace(_uploadUrlTemplate)
                    ? _uploadUrlTemplate.Replace("${FolderName}", effectiveFolder)
                    : $"{_cdnBaseUrl}/{effectiveFolder}".TrimEnd('/');

                var targetUrl = $"{uploadBase}/{uniqueFileName}";

                // Prepare request
                using var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var request = new HttpRequestMessage(HttpMethod.Put, targetUrl)
                {
                    Content = content
                };

                // Access key added as required by Bunny storage
                if (!string.IsNullOrWhiteSpace(_accessKey))
                    request.Headers.Add("AccessKey", _accessKey);

                var res = await _http.SendAsync(request);

                if (res.IsSuccessStatusCode)
                {
                    return targetUrl;
                }

                var body = string.Empty;
                try { body = await res.Content.ReadAsStringAsync(); } catch { }
                throw new InvalidOperationException($"CDN upload failed. Status: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload file '{fileName}' to CDN.", ex);
            }
        }
    }
}
