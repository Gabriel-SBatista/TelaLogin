using Login.Core.Presenter;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Login.Core.Services.RabbitMQServices
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EmailQueueService(string rabbitMqConnectionString)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(rabbitMqConnectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "email_queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void EnqueueEmail(Email email)
        {
            var message = JsonSerializer.Serialize(email);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "email_queue",
                                  basicProperties: null,
                                  body: body);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
