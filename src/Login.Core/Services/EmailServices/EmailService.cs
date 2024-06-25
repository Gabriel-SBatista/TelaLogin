using Login.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
        }

        public async Task<bool> SendEmailAsync(string addressee, Email email, CancellationToken cancellationToken = default)
        {
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

                        message.To.Add(addressee);

                        message.Subject = email.Subject;
                        message.Body = email.Body;

                        await smtpClient.SendMailAsync(message);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Email WriteEmail(int userId)
        {
            var email = new Email();

            email.Subject = "Email de confirmação de cadastro";
            email.Body = $"Confirme sua conta acessando o link: https://localhost:5209/api/users/confirm-email/{userId}";

            return email;
        }
    }
}
