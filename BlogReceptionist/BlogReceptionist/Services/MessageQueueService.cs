using BlogReceptionist.Controllers;
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
        public ConcurrentDictionary<int, QueryResult> resultQueue = new();
        public ManualResetEvent arrivalEvent = new(false);
        public string QName;
        public Dictionary<int, Thread> transIDList = new();
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
                QueryResult? blogs = null;
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                try
                {
                    blogs = JsonSerializer.Deserialize<QueryResult>(message);
                }
                catch (Exception)
                {
                    blogs = null;
                }

                if (blogs is null)
                    return;
                if (blogs.blogs is null)
                    return;
                lock (transIDList)
                {
                    if (transIDList.TryGetValue(blogs.transID, out var thread))
                    {
                        resultQueue.TryAdd(blogs.transID, blogs);
                        arrivalEvent.Set();
                        Thread.Sleep(2);
                        arrivalEvent.Reset();
                    }
                }

                //// this should be rare case
                //if (!BlogController.transIDList.ContainsKey(blogs.transID))
                //{
                //    resultQueue.TryRemove(blogs.transID, out var failedAdd);
                //}
            };
            await channel.BasicConsumeAsync(QName, autoAck: false, consumer: consumer);
        }

        public async Task GetAll(int transaction)
        {
            var message = $"{QName} {transaction} GETALL";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "HiQ",
                body: body);
        }

        public async Task GetByOwner(int transaction, long owner)
        {
            var message = $"{QName} {transaction} {owner}";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "HiQ",
                body: body);
        }
    }

    public class QueryResult
    {
        public int transID {  get; set; }
        public List<Blog>? blogs { get; set; }
    }
}
