using System;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Api.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddAppSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = new AppSettings();
            new ConfigureFromConfigurationOptions<AppSettings>(appSettingsSection).Configure(appSettings);
            services.Add(new ServiceDescriptor(typeof(AppSettings), appSettings));
        }

        public static void AddEmailSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var appSettingsSection = configuration.GetSection("EmailSettings");
            var emailSettings = new EmailSettings();
            new ConfigureFromConfigurationOptions<EmailSettings>(appSettingsSection).Configure(emailSettings);
            services.Add(new ServiceDescriptor(typeof(EmailSettings), emailSettings));
        }
    }
}
