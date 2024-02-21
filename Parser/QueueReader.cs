using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DbContext.Database.Models;
using Redis;

namespace Parser;

public class QueueReader
{
    public static async Task SubscriptionQueueRead(Data data, RedisDb redisDb)
    {
        while (true)
        {
            var subscribeData = await redisDb.GetSubscribeAsync();
            if (subscribeData.IsSome)
            {
                var subscribe = subscribeData.Unwrap();
                var apartmentDb = new ApartmentDb()
                {
                    Id = subscribe.Apartment.Id, 
                    Url = subscribe.Apartment.Url, 
                    Price = subscribe.Apartment.Price
                };
                var subscriberDb = new SubscriberDb()
                {
                    Id = subscribe.Subscriber.Id,
                    Email = subscribe.Subscriber.Email,
                    Apartments = [apartmentDb]
                };
                data.AddEntry(subscriberDb, apartmentDb);
            }
        }
    }
}