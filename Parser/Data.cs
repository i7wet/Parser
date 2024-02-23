using System.Collections.Concurrent;
using DbContext.Database;
using DbContext.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Parser;

public class Data
{


   private Data( ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>> subscribersByApartment)
   {
      SubscribersByApartment = subscribersByApartment;
   }

   public static async Task<Data> CreateData(TestDbContext testDbContext)
   {
      var data = new Data( await ReadDb(testDbContext));
      return data;
   }

   public ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>> SubscribersByApartment { get; }
   
   static async Task<ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>>> ReadDb(TestDbContext testDbContext)
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

      return subscribersByApartment;
   }

   public void AddEntry(SubscriberDb subscriberDb, ApartmentDb apartmentDb)
   {
      if (SubscribersByApartment.TryGetValue(apartmentDb, out var subscribers))
      {
         if (!subscribers.Contains(subscriberDb))
         {
            subscribers.Add(subscriberDb);
         }
      }
      else
      {
         SubscribersByApartment.AddOrUpdate(apartmentDb,
            (apartmentDb) => { return new ConcurrentBag<SubscriberDb>() { subscriberDb }; },
            (apartmentDb, subscribersDb) =>
            {
               if(!subscribersDb.Contains(subscriberDb))
                  subscribersDb.Add(subscriberDb);
               return subscribersDb;
            });
      }
   }
}