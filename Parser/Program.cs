using DbContext.Database;
using DbContext.Database.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parser;
using RabbitMQ.Models;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<TestDbContext>(o =>
    {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UseSqlServer();
    });
    
    var assembly = typeof(Program).Assembly;
    x.AddConsumers(assembly);
    x.AddActivities(assembly);
    
    x.UsingRabbitMq((context, configurator) =>
    {
        configurator.ConfigureEndpoints(context);
        configurator.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });;
    });
});

serviceCollection.AddDbContext<TestDbContext>( x =>
{
    x.UseSqlServer("Server=.\\SQLEXPRESS;Database=test-db;Trusted_Connection=True;TrustServerCertificate=True;");
});
serviceCollection.AddSingleton<Data>(provider => Data.CreateData(provider.GetRequiredService<TestDbContext>()).Result);
var serviceProvider = serviceCollection.BuildServiceProvider();
await using var scope = serviceProvider.CreateAsyncScope();
var testDbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
var busControl = serviceProvider.GetRequiredService<IBusControl>();
await busControl.StartAsync();
var data = scope.ServiceProvider.GetRequiredService<Data>();
var publishEndPoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();


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
                    await WriteDataInMessageQueue(entry.Key);
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


async Task WriteDb(ApartmentDb apartment)
{
    testDbContext.Apartments.Update(apartment);
    await testDbContext.SaveChangesAsync();
}

async Task WriteDataInMessageQueue(ApartmentDb apartment)
{
    if (data.SubscribersByApartment.TryGetValue(apartment, out var subscriberDbs))
    {
        foreach (var sub in subscriberDbs)
        {
            var message = new Message(sub.Email, apartment.Url, apartment.Price);
            await publishEndPoint.Publish(message);
        }
    }
}