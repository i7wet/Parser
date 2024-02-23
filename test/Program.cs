using DbContext.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using test;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddLocalization();
builder.Services.AddDbContext<TestDbContext>( x => x.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddMassTransit(x =>
    {
        x.AddEntityFrameworkOutbox<TestDbContext>(o =>
        {
            o.QueryDelay = TimeSpan.FromSeconds(5);
            o.UseSqlServer().UseBusOutbox();
        });
        x.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            configurator.ConfigureEndpoints(context);
        });
    }
);
// builder.Services.AddTransient<RedisDb>();
builder.Services.AddTransient<IConverter, Converter>();

var app = builder.Build();
// app.Services.UseSimpleInjector(container);
// builder.Services.Verify(VerificationOption.VerifyAndDiagnose);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);
app.Run();