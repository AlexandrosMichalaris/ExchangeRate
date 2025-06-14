using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExchangeRate.Infrastructure.Configuration.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Required to find appsettings.json
            .AddJsonFile("appsettings.json")
            //.AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Get the connection string from configuration
        var connectionString = configuration.GetConnectionString("DataCenter");

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DatabaseContext(optionsBuilder.Options);
    }
}