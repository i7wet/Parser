using System.Text.Json;
using Redis.Models;
using StackExchange.Redis;
using Utilities;

namespace Redis;

public class RedisDb
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDatabase _redisDb;
    private readonly string _keyForQueue = "MessageQueue";

    public RedisDb()
    {
        _multiplexer = ConnectionMultiplexer.Connect("localhost:10001");
        _redisDb = _multiplexer.GetDatabase();
    }

    public async Task EnqueueMessageAsync(Message message)
    {
        try
        {
            await _redisDb.ListRightPushAsync(_keyForQueue, JsonSerializer.Serialize(message));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Не удалось записать данные в очередь.\n {e}");
        }
    }

    public async Task<Option<Message>> GetMessageAsync()
    {
        try
        {
            var messageJson = await _redisDb.ListLeftPopAsync(_keyForQueue);
            if (messageJson.IsNull)
                return Option.None;
            var redisMessage = JsonSerializer.Deserialize<Message>(messageJson);
            return redisMessage;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Не удалось получить данные из очереди.\n {e}");
            return Option.None;
        }
    }
}