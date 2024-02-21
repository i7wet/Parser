using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using SimpleInjector.Lifestyles;


var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
container.Options.DefaultLifestyle = Lifestyle.Scoped;
container.Options.ResolveUnregisteredConcreteTypes = false;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddLocalization();
builder.Services.AddSimpleInjector(container, options =>
{
    // AddAspNetCore() wraps web requests in a Simple Injector scope and
    // allows request-scoped framework services to be resolved.
    options.AddAspNetCore()
        .AddControllerActivation();

    // Optionally, allow application components to depend on the non-generic
    // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
    // (Microsoft.Extensions.Localization) abstractions.
    options.AddLogging();
    options.AddLocalization();
});
var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
    .UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    .Options;
container.Register<TestDbContext>(() => new TestDbContext(dbContextOptions));
container.Register<HttpClient>(() => new HttpClient());

var app = builder.Build();
app.Services.UseSimpleInjector(container);
container.Verify(VerificationOption.VerifyAndDiagnose);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);
app.Run();