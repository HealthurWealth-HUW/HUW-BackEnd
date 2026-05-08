using HealthUrWelath.Application.Notifications.Interfaces;

namespace HealthUrWealth.Infrastructure.Notifications.Templates
{
    public sealed class TemplateRenderer : ITemplateRenderer
    {
        public async Task<string> RenderEmailAsync(
            string emailTemplateKey,
            IDictionary<string, string> tokens)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Notifications",
                "Templates",
                "Email",
                $"{emailTemplateKey}.html");

            var template = await File.ReadAllTextAsync(path);

            foreach (var token in tokens)
                template = template.Replace($"##{token.Key}##", token.Value);

            return template;
        }

        public async Task<string> RenderSmsAsync(
            string templateKey,
            IDictionary<string, string> tokens)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Notifications",
                "Templates",
                "Sms",
                $"{templateKey}.txt");

            var template = await File.ReadAllTextAsync(path);

            foreach (var token in tokens)
                template = template.Replace($"##{token.Key}##", token.Value);

            return template;
        }
    }
}
