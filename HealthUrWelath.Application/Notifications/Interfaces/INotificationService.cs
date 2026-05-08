using HealthUrWelath.Application.Notifications.Models;

namespace HealthUrWelath.Application.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(NotificationRequest request);
    }
}
