using BlogReceptionist.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BlogReceptionist.Services;

public class MessageQueueService
{
    private ILogger<MessageQueueService> classLogger;
    public MessageQueueService(ILogger<MessageQueueService> logger)
    {
        channel = initStep1().Result;
        QName = initStep2().Result;
        initStep3().Wait();
        classLogger = logger;
    }

    #region InitializeMessageQueue quite boring setting up fields among async
    private async Task<IChannel> initStep1()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync();
        var channel1 = await connection.CreateChannelAsync();
        return channel1;
    }

    private async Task<string> initStep2()
    {
        var declaredQ = await channel.QueueDeclareAsync(
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        return declaredQ.QueueName;
    }

    private async Task initStep3()
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnResponseMessageArrive;
        await channel.BasicConsumeAsync(QName, autoAck: false, consumer: consumer);
    }
    #endregion

    private async Task OnResponseMessageArrive(object model, BasicDeliverEventArgs ea)
    {
        byte[] body = ea.Body.ToArray();
        string messageStr = Encoding.UTF8.GetString(body);
        QueryIDBody? message = null;
        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        try
        {
            message = JsonSerializer.Deserialize<QueryIDBody>(messageStr);
        }
        catch (Exception)
        {
        }
        // json issue
        if (message is null)
            return;
        // json issue
        if (message.blogs is null)
            return;

        classLogger.LogInformation($"{ea.RoutingKey} [x] Received {messageStr}");
        lock (transIDList)
        {
            if (transIDList.TryGetValue(message.transID, out var threadWakeUp))
            {
                resultQueue.TryAdd(message.transID, message);
                threadWakeUp.Set();
            }
        }
    }

    public List<Blog>? Get() => GetCommand();

    public List<Blog>? Get(long owner) => GetCommand(owner.ToString());

    // Similar to UDP programming without built-in stream id and sync call
    public List<Blog>? GetCommand(string thirdPart = "GETALL")
    {
        int transID = Interlocked.Add(ref indexer, 1);
        using var arrivalEvent = new AutoResetEvent(false);
        using var repeatTimer = new System.Timers.Timer(RETRYTIMEOUT);

        lock (transIDList)
        {
            transIDList.Add(transID, arrivalEvent);
        }

        var message = $"{QName} {transID} {thirdPart}";
        var body = Encoding.UTF8.GetBytes(message);

        DateTime timeoutbase = DateTime.Now;

        channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "HiQ",
            body: body);
        repeatTimer.Elapsed += async (object? sender, System.Timers.ElapsedEventArgs e) =>
        {
            if (DateTime.Now - timeoutbase < TIMEOUTLIMIT)
            {
                classLogger.LogWarning($"Send one extra request to messageQ. Time(ms): {(DateTime.Now - timeoutbase).TotalMilliseconds}");
                await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: "HiQ",
                        body: body);
            }
            else
            {
                repeatTimer.Stop();
                return;
            }
        };
        repeatTimer.Start();
        arrivalEvent.WaitOne(TIMEOUTLIMIT);
        repeatTimer.Stop();
        if (!resultQueue.TryGetValue(transID, out QueryIDBody? value))
        {
            return null;
        }
        List<Blog>? blogs = value.blogs;
        lock (transIDList)
        {
            transIDList.Remove(transID);
        }
        resultQueue.TryRemove(transID, out _);
        return blogs;
    }
    public class QueryIDBody
    {
        public int transID { get; set; }
        public List<Blog>? blogs { get; set; }
    }

    public IChannel channel;
    public string QName;
    public Dictionary<int, AutoResetEvent> transIDList = [];
    public ConcurrentDictionary<int, QueryIDBody> resultQueue = new();

    // const
    public TimeSpan RETRYTIMEOUT => TimeSpan.FromSeconds(5);
    public TimeSpan TIMEOUTLIMIT => TimeSpan.FromSeconds(28);

    // auto increment
    private int indexer = 0;
}
