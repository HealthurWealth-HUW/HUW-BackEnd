using HealthUrWelath.Application.Notifications.Models;

namespace HealthUrWelath.Application.Notifications.Interfaces
{
    public interface INotificationService
    {
        /// <returns>true if the notification was dispatched to all requested channels; false if it failed (the failure is logged internally).</returns>
        Task<bool> SendAsync(NotificationRequest request);
    }
}
