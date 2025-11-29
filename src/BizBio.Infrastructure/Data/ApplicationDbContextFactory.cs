using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace BizBio.Infrastructure.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext to support EF Core migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BizBio.API"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json. " +
                "Please ensure the connection string is configured in the API project's appsettings.json file.");
        }

        // Create DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use MariaDB or MySQL version depending on your database
        // For MariaDB 10.5+, use: new MariaDbServerVersion(new Version(10, 5, 0))
        // For MySQL 8.0+, use: new MySqlServerVersion(new Version(8, 0, 21))
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
        // Uncomment the line below if using MariaDB instead:
        // var serverVersion = new MariaDbServerVersion(new Version(10, 5, 0));

        optionsBuilder.UseMySql(
            connectionString,
            serverVersion,
            mySqlOptions => mySqlOptions.EnableRetryOnFailure());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
