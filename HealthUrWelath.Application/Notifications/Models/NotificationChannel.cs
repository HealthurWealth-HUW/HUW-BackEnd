namespace HealthUrWelath.Application.Notifications.Models
{
    [Flags]
    public enum NotificationChannel
    {
        None = 0,
        Email = 1,
        Sms = 2,
        Both = Email | Sms
    }
}
