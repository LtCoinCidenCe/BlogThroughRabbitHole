using BlogServer.DBContexts;
using BlogServer.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using BlogServer.Models;
using BlogServer.Utilities;
using Microsoft.Extensions.Hosting;

try {File.Delete("healthy.txt");}
catch (Exception){}

var host = Host.CreateDefaultBuilder().Build();

var factory = new ConnectionFactory { HostName = Env.MQURL };
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
    var order = message.Split(' ');
    var QName = order[0];
    var transIDstr = order[1];
    var id = order[2];
    Console.WriteLine($" [x] Received {message}");

    int.TryParse(transIDstr, out var transID);
    //await Task.Delay(2000);
    List<Blog> blogs;
    if (long.TryParse(id, out long owner))
        blogs = blogService.GetBlogsByOwner(owner);
    else
        blogs = blogService.GetAll();

    blogs.ForEach(b => {
        Console.WriteLine(b.Content);
    });
    Dictionary<string, object> tobe = new Dictionary<string, object>
    {
        { "transID", transID },
        { "blogs", blogs }
    };

    string str = JsonSerializer.Serialize(tobe);
    var resultBytes = Encoding.UTF8.GetBytes(str);

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: QName,
        body: resultBytes);
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync("HiQ", autoAck: false, consumer: consumer);

Thread.Sleep(1 * 1000);
File.WriteAllText("healthy.txt", DateTime.Now.ToString() + " RabbitMQ started.");
var cleanHealthUp = Task.Run(() =>
{
    Thread.Sleep(30 * 1000);
    try { File.Delete("healthy.txt"); }
    catch (Exception) { }
});
Console.WriteLine("RabbitMQ online");

host.Run();
