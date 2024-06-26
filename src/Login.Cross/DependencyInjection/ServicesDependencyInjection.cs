using Login.Core.Services.EmailServices;
using Login.Core.Services.Hasher;
using Login.Core.Services.RabbitMQServices;
using Login.Core.Services.TokenService;
using Login.Core.Services.UserServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Cross.DependencyInjection
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IEmailService, EmailService>(provider =>
            {
                string smtpServer = configuration.GetSection("SMTP").GetRequiredSection("Server").Value;
                int smtpPort = int.Parse(configuration.GetSection("SMTP").GetRequiredSection("Porta").Value);
                string smtpUsername = configuration.GetSection("SMTP").GetRequiredSection("Username").Value;
                string smtpPassword = configuration.GetSection("SMTP").GetRequiredSection("Password").Value;
                var logger = provider.GetRequiredService<ILogger<EmailService>>();

                return new EmailService(smtpServer, smtpPort, smtpUsername, smtpPassword, logger);
            });
            services.AddScoped<ITokenService, TokenService>(provider => {
                string stringKey = configuration.GetSection("JWTToken").GetSection("StringKey").Value;

                return new TokenService(stringKey);
            });

            var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMQ");
            services.AddSingleton<IEmailQueueService>(sp => new EmailQueueService(rabbitMqConnectionString));

            services.AddHostedService<EmailConsumerService>();

            return services;
        }
    }
}
