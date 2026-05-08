using HealthUrWelath.Application.BlueDart.Dtos;
using HealthUrWelath.Application.BlueDart.Interfaces;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace HealthUrWealth.Infrastructure.BlueDart
{
    public class BluedartService : IBluedartService
    {
        private readonly HttpClient _httpClient;
        private readonly BluedartSettings _settings;

        public BluedartService(HttpClient httpClient, IOptions<BluedartSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<EddDto> GetEDD(string pincode)
        {
            var token = await GetJwtToken(); // reusable

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{_settings.BaseUrl}/in/transportation/transit/v1/GetDomesticTransitTimeForPinCodeandProduct");

            request.Headers.Add("JWTToken", token);

            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
            // Bluedart expects dates in the format: /Date(<milliseconds since epoch>)/
            var pPudateValue = $"/Date({new DateTimeOffset(indiaTime).ToUnixTimeMilliseconds()})/";

            var body = new
            {
                pPinCodeTo = pincode,
                pPickupTime =  _settings.PickupTime,
                pPinCodeFrom = _settings.OriginPincode,
                pProductCode = _settings.ProductCode,
                pPudate = pPudateValue,
                pSubProductCode = _settings.SubProductCode,
                profile = new
                {
                    Api_type = _settings.ApiType,
                    LicenceKey = _settings.LicenceKey,
                    LoginID = _settings.LoginId
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode && string.IsNullOrWhiteSpace(json))
            {
                // Use the HTTP reason/status instead.
                throw new Exception($"Transit API failed: {response.ReasonPhrase} (Status: {(int)response.StatusCode})");
            }

            //sample response

            //{"GetDomesticTransitTimeForPinCodeandProductResult":
            //{"AdditionalDays":0,"ApexAdditionalDays":0,"Area":"DEL","CityDesc_Destination":"NEW DELHI",
            //"CityDesc_Origin":"HYDERABAD","EDLMessage":"N","ErrorMessage":"Valid",
            //"ExpectedDateDelivery":"14-APR-26","ExpectedDatePOD":"14-APR-26",
            //"GroundAdditionalDays":0,"IsError":false,"ServiceCenter":"MOC"}}

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            EddResponse result = null;
            try
            {
                result = JsonSerializer.Deserialize<EddResponse>(json, options);
            }
            catch
            {
                // ignore deserialize errors and try alternate shape below
            }

            var transit = result?.GetDomesticTransitTimeForPinCodeandProductResult;

            // If transit is null, Bluedart may have returned an error payload like:
            // { "status":400, "title":"Bad Request", "error-response":[{...}] }
            // Try to extract the first element of "error-response" and map it to TransitResult.
            if (transit == null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("error-response", out var errArray) && errArray.ValueKind == JsonValueKind.Array && errArray.GetArrayLength() > 0)
                    {
                        var first = errArray[0];
                        transit = JsonSerializer.Deserialize<TransitResult>(first.GetRawText(), options);
                    }
                    else if (root.TryGetProperty("GetDomesticTransitTimeForPinCodeandProductResult", out var normal))
                    {
                        transit = JsonSerializer.Deserialize<TransitResult>(normal.GetRawText(), options);
                    }
                }
                catch
                {
                    // swallow - we'll return empty dto if parsing fails
                }
            }

            var dto = new EddDto();

            if (transit != null)
            {
                dto.DestinationCity = transit.CityDesc_Destination;
                dto.OriginCity = transit.CityDesc_Origin;
                dto.IsError = transit.IsError;
                dto.ErrorMessage = transit.ErrorMessage;

                if (!string.IsNullOrWhiteSpace(transit.ExpectedDateDelivery) &&
                    DateTime.TryParseExact(transit.ExpectedDateDelivery, "dd-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    dto.DeliveryDate = parsed;
                }
                if (dto.IsError)
                {
                    // reuse indiaTime computed earlier and default to +4 days
                    dto.DeliveryDateWhenNoEDD = indiaTime.AddDays(4);
                }
            }

            return dto;
        }

        // 🔥 REUSABLE PRIVATE METHOD
        private async Task<string> GetJwtToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_settings.BaseUrl}/in/transportation/token/v1/login");

            request.Headers.Add("ClientID", _settings.ClientId);
           // request.Headers.Add("clientSecret", _settings.ClientSecret);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Token API failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("JWTToken", out var tokenElement))
            {
                throw new Exception("JWTToken not found");
            }

            return tokenElement.GetString();
        }
    }
}
