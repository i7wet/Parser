using DbContext.Database.Models;
using MassTransit;
using RabbitMQ.Models;

namespace Parser.Consumers;

public class SubscribeCreatedConsumer : IConsumer<Subscribe>
{
    private readonly Data _data;

    public SubscribeCreatedConsumer(Data data)
    {
        _data = data;
    }

    public Task Consume(ConsumeContext<Subscribe> context)
    {
        var apartmentDb = new ApartmentDb()
        {
            Id = context.Message.Apartment.Id, 
            Url = context.Message.Apartment.Url, 
            Price = context.Message.Apartment.Price
        };
        var subscriberDb = new SubscriberDb()
        {
            Id = context.Message.Subscriber.Id,
            Email = context.Message.Subscriber.Email,
            Apartments = [apartmentDb]
        };
        _data.AddEntry(subscriberDb, apartmentDb);
        return Task.CompletedTask;
    }
}
