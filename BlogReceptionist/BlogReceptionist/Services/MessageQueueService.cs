using BlogReceptionist.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BlogReceptionist.Services
{
    public class MessageQueueService
    {
        public IChannel channel;
        public ConcurrentQueue<List<Blog>> resultQueue = new();
        public AutoResetEvent arrivalEvent = new(false);
        public string QName;
        public MessageQueueService()
        {
            InitializeMessageQueue().Wait();
        }

        private async Task InitializeMessageQueue()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();
            var declaredQ = await channel.QueueDeclareAsync(
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            QName = declaredQ.QueueName;
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"{ea.RoutingKey} [x] Received {message}");
                List<Blog>? blogs = null;
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                try
                { 
                    blogs = JsonSerializer.Deserialize<List<Blog>>(message);
                }
                catch (Exception)
                {
                    blogs = null;
                }
                
                if (blogs is null)
                {
                }
                else
                {
                    resultQueue.Enqueue(blogs);
                    arrivalEvent.Set();
                }
            };
            await channel.BasicConsumeAsync(QName, autoAck: false, consumer: consumer);
        }

        public async Task GetAll()
        {
            var message = $"{QName} GETALL";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "HiQ",
                body: body);
        }
    }
}
