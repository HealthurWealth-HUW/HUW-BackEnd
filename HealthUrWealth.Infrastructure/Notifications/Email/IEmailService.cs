using HealthUrWelath.Application.Notifications.Models;

namespace HealthUrWelath.Infrastructure.Notifications.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
