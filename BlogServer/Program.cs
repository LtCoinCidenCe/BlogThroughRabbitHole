using BlogServer.DBContexts;
using BlogServer.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
Console.WriteLine(" [*] Waiting for messages.");
var consumer = new AsyncEventingBasicConsumer(channel);


BlogService blogService = new BlogService(new BlogContext());
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    var blogs = blogService.GetAll();
    blogs.ForEach(b => Console.WriteLine(b.Content));
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
