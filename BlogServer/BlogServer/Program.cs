using BlogServer.DBContexts;
using BlogServer.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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
    var order = message.Split(' ');
    var QName = order[0];
    var transIDstr = order[1];
    Console.WriteLine($" [x] Received {message}");

    int.TryParse(transIDstr, out var transID);
    //await Task.Delay(2000);
    var blogs = blogService.GetAll();

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

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
