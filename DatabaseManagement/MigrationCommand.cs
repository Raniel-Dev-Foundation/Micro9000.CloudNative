using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace DatabaseManagement;
internal sealed class MigrationCommand : Command<MigrationCommand.Settingns>
{
    private readonly IConfiguration _configuration;

    public MigrationCommand(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public sealed class Settingns : CommandSettings
    {
        [Description("Override connection string from config.")]
        [CommandOption("-c|--connection-string")]
        public string? ConnectionString { get; init; }

        [Description("Create the database if it does not exists.")]
        [CommandOption("-f|--force-ensure-database")]
        [DefaultValue(true)]
        public bool ForceEnsureDatabase { get; init; }

        [Description("Create mock data.")]
        [CommandOption("-m|--mock-data")]

#if DEBUG
        [DefaultValue(true)]
#else
        [DefaultValue(false)]
#endif
        public bool MockData { get; init; }

        [Description("Create seed data.")]
        [CommandOption("-s|--seed-data")]
        [DefaultValue(true)]
        public bool SeedData { get; init; }
    }

    public override int Execute(CommandContext context, Settingns settings)
    {
        var connectionString = settings.ConnectionString ?? _configuration.GetConnectionString("Micro9000_DbConnection_migration");
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        try
        {
            DatabaseUpgrader.RunDbUpgradeActivities(connectionString, settings.ForceEnsureDatabase, settings.MockData, settings.SeedData);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
            throw;
        }

        return 0;
    }



}
