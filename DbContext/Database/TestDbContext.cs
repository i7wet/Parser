using DbContext.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DbContext.Database;

public class TestDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<SubscriberDb> Subscribers { get; set; }
    public DbSet<ApartmentDb> Apartments { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
        // Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=test-db;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}

public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var fileSettingsName = "appsettings.json";
        if (Path.Exists(Directory.GetCurrentDirectory() + "\\appsettings.Development.json"))
            fileSettingsName = "appsettings.Development.json";
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(fileSettingsName, optional: false);

        IConfiguration config = builder.Build();

        var connectionString = config.GetConnectionString("Default");
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new TestDbContext(optionsBuilder.Options);
    }
}