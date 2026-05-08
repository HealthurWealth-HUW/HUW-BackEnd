using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HealthUrWealth.Infrastructure.Notifications.Sms
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<SmsService> _logger;

        public SmsService(
            HttpClient client,
            IConfiguration config,
            ILogger<SmsService> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendAsync(
            string mobile,
            string message,
            string templateId)
        {
            var apiUrl = _config["SmsGateway:Url"];
            var username = _config["SmsGateway:Username"];
            var senderId = _config["SmsGateway:SenderId"];
            var entityId = _config["SmsGateway:EntityId"];
            var apiKey = _config["SmsGateway:ApiKey"];

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var payload = new
                {
                    sendSMS = "1",
                    username = username,
                    sender_id = senderId,
                    msg = message,
                    mobile = mobile,
                    template_id = templateId,
                    entity_id = entityId,
                    apikey = apiKey
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(apiUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("SMS Gateway Response: {Response}", result);

                var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(result);

                if (smsResponse?.result == "success")
                {
                    _logger.LogInformation("SMS sent to {Mobile}", mobile);
                    return true;
                }

                _logger.LogError(
                    "SMS failed. Mobile={Mobile}, Error={Error}",
                    mobile,
                    smsResponse?.error);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "SMS exception. Mobile={Mobile}",
                    mobile);

                throw; // DO NOT swallow — let app know
            }
        }
    }
}