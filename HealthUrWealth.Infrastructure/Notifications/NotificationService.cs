using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HealthUrWealth.Infrastructure.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITemplateRenderer _templateRenderer;

        public NotificationService(
           IServiceProvider serviceProvider,
            ITemplateRenderer templateRenderer)
        {
            _serviceProvider = serviceProvider;
            _templateRenderer = templateRenderer;
        }

        public async Task SendAsync(NotificationRequest request)
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
            }
            catch (Exception ex)
            {

            }
        }





    }
}
