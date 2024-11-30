using BlogReceptionist.Models;
using BlogReceptionist.Utilities;
using StackExchange.Redis;
using System.Text.Json;

namespace BlogReceptionist.Services;
public class RedisService
{
    ConnectionMultiplexer redis;
    IDatabase db;
    ILogger<RedisService> logger;
    public RedisService(ILogger<RedisService>DIlogger)
    {
        logger = DIlogger;
        redis = ConnectionMultiplexer.Connect(Env.REDISURL, configure: (config) =>
        {
            config.AbortOnConnectFail = false;
            config.ConnectTimeout = 2000;
            config.SyncTimeout = 2000;
            config.AsyncTimeout = 2000;
        });
        redis.ConnectionFailed += (sender, args) =>
        {
            logger.LogWarning("Redis failed");
        };
        redis.ConnectionRestored += (sender, args) =>
        {
            logger.LogWarning("Redis restored");
        };
        db = redis.GetDatabase();
        
    }

    public void setBloglist()
    {
        string value = "abcdefg";
        db.StringSet("mykey", value);
    }

    public void setUser(User user)
    {
        string value = JsonSerializer.Serialize(user
            , options: new JsonSerializerOptions() { WriteIndented = false });

        try
        {
            db.StringSetAsync($"User {user.ID}", value, TimeSpan.FromHours(2));
        }
        catch (RedisConnectionException ex)
        {
            logger.LogCritical(ex.Message);
            return;
        }
    }

    public User? getUser(long id)
    {
        RedisValue result;
        try
        {
            result = db.StringGet($"User {id}");
        }
        catch (RedisConnectionException ex)
        {
            logger.LogCritical(ex.Message);
            return null;
        }
        if (result.IsNullOrEmpty)
            return null;
        User? user = null;
        user = JsonSerializer.Deserialize<User>(result.ToString());
        return user;
    }
}
