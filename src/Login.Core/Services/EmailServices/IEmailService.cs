using Login.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.EmailServices
{
    public interface IEmailService
    {
        Task <bool> SendEmailAsync(string addressee, Email email, CancellationToken cancellationToken = default);
        Email WriteEmail(int userId);
    }
}
