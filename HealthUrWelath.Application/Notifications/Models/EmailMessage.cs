namespace HealthUrWelath.Application.Notifications.Models
{
    public sealed class EmailMessage
    {
        public string To { get; init; } = default!;
        public List<string>? Bcc { get; init; }
        public string Subject { get; init; } = default!;
        public string Body { get; init; } = default!;
        public bool IsHtml { get; init; } = true;
    }
}
