using MassTransit;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<EmailService.EmailService>();
serviceCollection.AddMassTransit(x =>
{
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

var serviceProvider = serviceCollection.BuildServiceProvider();
await using var scope = serviceProvider.CreateAsyncScope();
var busControl = serviceProvider.GetRequiredService<IBusControl>();
await busControl.StartAsync();
await Task.Delay(-1);

