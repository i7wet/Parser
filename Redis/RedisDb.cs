using System.Text.Json;
using Redis.Models;
using StackExchange.Redis;
using Utilities;

namespace Redis;

public class RedisDb
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDatabase _redisDb;
    private readonly string _keyForMessageQueue = "MessageQueue";
    private readonly string _keyForSubscribeQueue = "MessageQueue";

    public RedisDb()
    {
        _multiplexer = ConnectionMultiplexer.Connect("localhost:10001");
        _redisDb = _multiplexer.GetDatabase();
    }

    public async Task EnqueueMessageAsync(Message message)
    {
        try
        {
            await _redisDb.ListRightPushAsync(_keyForMessageQueue, JsonSerializer.Serialize(message));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Не удалось записать данные для сообщения в очередь.\n {e}");
        }
    }

    public async Task<Option<Message>> GetMessageAsync()
    {
        try
        {
            var messageJson = await _redisDb.ListLeftPopAsync(_keyForMessageQueue);
            if (messageJson.IsNull)
                return Option.None;
            var message = JsonSerializer.Deserialize<Message>(messageJson);
            return message;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Не удалось получить данные из очереди.\n {e}");
            return Option.None;
        }
    }

    public async Task EnqueueSubscribeAsync(Subscribe subscribe)
    {
        try
        {
            await _redisDb.ListRightPushAsync(_keyForSubscribeQueue, JsonSerializer.Serialize(subscribe));
        }
        catch (Exception e)
        {
            throw new Exception($"Ключ бд - {_keyForSubscribeQueue}.\nНе удалось записать данные о новой подписке в очередь.\n{e}");
        }
    }
    
    public async Task<Option<Subscribe>> GetSubscribeAsync()
    {
        try
        {
            var subscribeJson = await _redisDb.ListLeftPopAsync(_keyForSubscribeQueue);
            if (subscribeJson.IsNull)
                return Option.None;
            var subscribe = JsonSerializer.Deserialize<Subscribe>(subscribeJson);
            return subscribe;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ключ бд - {_keyForSubscribeQueue}.\nНе удалось получить данные из очереди.\n{e}");
            return Option.None;
        }
    }
}