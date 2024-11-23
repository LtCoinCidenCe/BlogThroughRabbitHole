using BlogServer.DBContexts;
using BlogServer.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(
    queue: "HiQ",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);
await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
Console.WriteLine(" [*] Waiting for messages.");

BlogService blogService = new BlogService(new BlogContext());

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    await Task.Delay(2000);
    var blogs = blogService.GetAll();
    blogs.ForEach(b => Console.WriteLine(b.Content));
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync("HiQ", autoAck: false, consumer: consumer);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
