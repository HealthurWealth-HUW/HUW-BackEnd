using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HealthUrWealth.Infrastructure.Notifications.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(
            IConfiguration config,
            ILogger<EmailService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var apiKey = _config["BrevoEmail:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Brevo ApiKey missing");

            var client = _httpClientFactory.CreateClient("BrevoClient");//program.cs has this config

            // Ensure clean headers (important)
            client.DefaultRequestHeaders.Remove("api-key");
            client.DefaultRequestHeaders.Add("api-key", apiKey);

            var body = new
            {
                sender = new
                {
                    email = _config["BrevoEmail:FromEmail"],
                    name = _config["BrevoEmail:FromName"]
                },
                to = new[]
                {
                new { email = message.To }
            },
                subject = message.Subject,
                htmlContent = message.IsHtml ? message.Body : null,
                textContent = message.IsHtml ? null : message.Body,
                bcc = message.Bcc?.Select(b => new { email = b }).ToArray()
            };

            try
            {
                var response = await client.PostAsJsonAsync("smtp/email", body);
           
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Brevo email failed: {Error}", error);
                    throw new Exception($"Brevo failed: {error}");
                }

                _logger.LogInformation("Email sent successfully to {To}", message.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending email to {To}", message.To);
                throw;
            }
        }
    }
}
