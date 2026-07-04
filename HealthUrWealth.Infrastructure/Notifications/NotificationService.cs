using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HealthUrWealth.Infrastructure.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
           IServiceProvider serviceProvider,
            ITemplateRenderer templateRenderer,
            ILogger<NotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _templateRenderer = templateRenderer;
            _logger = logger;
        }

        public async Task<bool> SendAsync(NotificationRequest request)
        {
            try
            {
                var tasks = new List<Task>();

                // -------- EMAIL --------
                if (request.Channel.HasFlag(NotificationChannel.Email)
                    && !string.IsNullOrWhiteSpace(request.Email))
                {
                    var emailBody = await _templateRenderer.RenderEmailAsync(
                        request.EmailTemplateKey,
                        request.Tokens);

                    var emailMessage = new EmailMessage
                    {
                        To = request.Email,
                        Subject = request.Subject ?? "Notification",
                        Body = emailBody,
                        IsHtml = true
                    };
                    var emailService = _serviceProvider.GetRequiredService<IEmailService>();
                    tasks.Add(emailService.SendAsync(emailMessage));
                }

                // -------- SMS --------
                if (request.Channel.HasFlag(NotificationChannel.Sms)
                    && !string.IsNullOrWhiteSpace(request.Mobile))
                {
                    var smsBody = await _templateRenderer.RenderSmsAsync(
                        request.EmailTemplateKey,
                        request.Tokens);

                    var smsService = _serviceProvider.GetService<ISmsService>();
                    tasks.Add(
                        smsService.SendAsync(
                            request.Mobile,
                            smsBody,
                            request.SmsTemplateKey
                        ));
                }

                if (tasks.Count > 0)
                    await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Notification dispatch failed. Channel={Channel} Mobile={Mobile} Email={Email} EmailTemplate={EmailTemplate} SmsTemplate={SmsTemplate}",
                    request.Channel,
                    request.Mobile,
                    request.Email,
                    request.EmailTemplateKey,
                    request.SmsTemplateKey);

                return false;
            }
        }





    }
}
