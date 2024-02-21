using System.Collections.Concurrent;
using DbContext.Database;
using DbContext.Database.Models;
using Microsoft.EntityFrameworkCore;
using Parser;
using Redis;
using Redis.Models;


var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
    .UseSqlServer("Server=.\\SQLEXPRESS;Database=test-db;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;
var testDbContext = new TestDbContext(dbContextOptions);
var data = await ReadDb();

var listener = new Thread(() => DataReception.HttpListen(data));
listener.Start();

var redis = new RedisDb();


while (true)
{
    try
    {
        foreach (var entry in data.SubscribersByApartment)
        {
            var parseResult = await Parser.Parser.Parse(entry.Key);
            if (!parseResult.IsError)
            {
                if (parseResult.Ok)
                {
                    await WriteDb(entry.Key);
                    await WriteDataInQueue(entry.Key);
                }
            }
            else
            {
                Console.WriteLine(parseResult.Error);
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

async Task<Data> ReadDb()
{
    var apartments = await testDbContext.Apartments.ToListAsync();
    var subscribers = await testDbContext.Subscribers.Include(subscriberDb => subscriberDb.Apartments).ToListAsync();
    ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>> subscribersByApartment = new();
    foreach (var apartment in apartments)
    {
        var confirmedSubscribers = new ConcurrentBag<SubscriberDb>();
        foreach (var subscriber in subscribers)
            if (subscriber.Apartments.Contains(apartment))
                confirmedSubscribers.Add(subscriber);

        subscribersByApartment.TryAdd(apartment, confirmedSubscribers);
    }

    return new Data(subscribersByApartment);
}

async Task WriteDb(ApartmentDb apartment)
{
    testDbContext.Apartments.Update(apartment);
    await testDbContext.SaveChangesAsync();
}

async Task WriteDataInQueue(ApartmentDb apartment)
{
    if (data.SubscribersByApartment.TryGetValue(apartment, out var subscriberDbs))
    {
        foreach (var sub in subscriberDbs)
        {
            var message = new Message(sub.Email, apartment.Url, apartment.Price);
            await redis.EnqueueMessageAsync(message);
        }
    }
}