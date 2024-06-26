using Login.Core.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.RabbitMQServices
{
    public interface IEmailQueueService : IDisposable
    {
        void EnqueueEmail(Email email);
    }
}
