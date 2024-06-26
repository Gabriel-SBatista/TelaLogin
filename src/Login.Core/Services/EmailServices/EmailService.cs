using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Login.Core.Presenter;
using Microsoft.Extensions.Logging;

namespace Login.Core.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly ILogger<EmailService> _logger;

        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, ILogger<EmailService> logger)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("starting sending email to {email}", email.To);

            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_smtpServer))
                {
                    smtpClient.Port = _smtpPort;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(_smtpUsername);

                        message.To.Add(email.To);

                        message.Subject = email.Subject;
                        message.Body = email.Body;

                        await smtpClient.SendMailAsync(message, cancellationToken);

                        _logger.LogInformation("email successfully sent to {email}", email.To);
                    }
                }

                return true;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "error sending email to {email}", email.To);
                return false;
            }
        }
    }
}
