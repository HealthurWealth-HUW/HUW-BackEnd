
namespace HealthUrWelath.Application.Notifications.Interfaces
{
    public interface ITemplateRenderer
    {
        Task<string> RenderEmailAsync(string templateKey, IDictionary<string, string> tokens);
        Task<string> RenderSmsAsync(string templateKey, IDictionary<string, string> tokens);
    }
}
