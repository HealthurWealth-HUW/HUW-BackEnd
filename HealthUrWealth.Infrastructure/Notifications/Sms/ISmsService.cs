namespace HealthUrWelath.Infrastructure.Notifications.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendAsync(string mobile, string message, string templateId);
    }
}
