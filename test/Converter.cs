using DbContext.Database.Models;
using Redis.Models;
using Utilities;

namespace test;

public class Converter : IConverter
{
    public Result<T1, Exception> Convert<T2, T1>(T2 t2) where T1 : class 
    {
        if (t2 is ApartmentDb apartmentDb && typeof(T1) == typeof(Apartment))
        {
            return (T1)(object)Convert(apartmentDb);
        }
        if (t2 is SubscriberDb subscriberDb  && typeof(T1) == typeof(Subscriber))
        {
            return (T1)(object)Convert(subscriberDb);
        }
        else
        {
            return new Exception($"Не удалось сконвертировать тип {typeof(T2)} в тип {typeof(T1)}");
        }
    }

    private Apartment Convert(ApartmentDb apartmentDb)
    {
        return new Apartment() { Id = apartmentDb.Id, Price = apartmentDb.Price, Url = apartmentDb.Url };
    }
    
    private Subscriber Convert(SubscriberDb subscriberDb)
    {
        return new Subscriber() { Id = subscriberDb.Id, Email = subscriberDb.Email };
    }
}