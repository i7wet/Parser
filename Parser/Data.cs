using System.Collections.Concurrent;
using DbContext.Database.Models;

namespace Parser;

public class Data
{
   public Data(ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>> subscribersByApartment)
   {
      SubscribersByApartment = subscribersByApartment;
   }

   public ConcurrentDictionary<ApartmentDb, ConcurrentBag<SubscriberDb>> SubscribersByApartment { get; set; }

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
            (db, dbs) =>
            {
               if(!dbs.Contains(subscriberDb))
                  dbs.Add(subscriberDb);
               return dbs;
            });
      }
   }
}