namespace HealthUrWelath.Application.Notifications.Models
{
    public sealed class NotificationRequest
    {
        public string EmailTemplateKey { get; init; } = default!;
        public NotificationChannel Channel { get; init; }

        public string? Email { get; init; }
        public string? Mobile { get; init; }

        public string? Subject { get; init; }
        public string? SmsTemplateKey { get; init; }

        public Dictionary<string, string> Tokens { get; init; }
            = new();
    }
}
