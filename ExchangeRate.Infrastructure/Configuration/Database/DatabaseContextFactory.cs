using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ExchangeRate.Infrastructure.Configuration.Database;

namespace ExchangeRate.Infrastructure.Configuration.Database;

// public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
// {
//     public DatabaseContext CreateDbContext(string[] args)
//     {
//         // Build configuration from appsettings.json
//         IConfigurationRoot configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json")
//             .Build();
//
//         // Read connection string
//         var connectionString = configuration.GetConnectionString("ExchangeRate");
//
//         var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
//         optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "exchange"));
//
//         return new DatabaseContext(optionsBuilder.Options);
//     }
// }