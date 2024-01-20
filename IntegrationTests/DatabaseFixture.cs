using Microsoft.Extensions.Configuration;
using Core.Data;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using DatabaseManagement;

namespace IntegrationTests;
public class DatabaseFixture : IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly CloudNativeDbContext _dbContext;
    private const string DatabaseConnectionStringConfigKey = "Micro9000_DbConnection";

    public DatabaseFixture()
    {
        var environmentName = Environment.GetEnvironmentVariable("Configuration") ?? "Development";

        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        _dbContext = CreateDbContext();
        var connString = _configuration.GetConnectionString(DatabaseConnectionStringConfigKey);
        DatabaseUpgrader.RunDbUpgradeActivities(connString!, true, false, true);
    }

    public CloudNativeDbContext CreateDbContext(ITestOutputHelper? logSensitiveDataTo = null)
    {
        return CreateDbContext(DatabaseConnectionStringConfigKey);
    }

    private CloudNativeDbContext CreateDbContext (string connectionStringName, ITestOutputHelper? logSensitiveDataTo = null)
    {
        var builder = new DbContextOptionsBuilder<CloudNativeDbContext>()
            .UseSqlServer(_configuration.GetConnectionString(connectionStringName)!, opt =>
            {
                opt.CommandTimeout(120); // Integration tests may run some long running delete commands, so make this larger
                opt.EnableRetryOnFailure();
            })
            .ConfigureWarnings(builder => builder
                .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
                .Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)
                .Ignore(SqlServerEventId.DecimalTypeDefaultWarning))
            .EnableSensitiveDataLogging(logSensitiveDataTo != null)
                .LogTo(m => logSensitiveDataTo?.WriteLine(m), LogLevel.Information);

        var options = builder.Options;
        return new CloudNativeDbContext(options);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
