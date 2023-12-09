using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics;

namespace DatabaseManagement.Commands;
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
        [DefaultValue(false)]
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
            // Only try and create database if we're not running in the release pipeline (In Azure DB it is already created for us by Terraform)
            if (settings.ForceEnsureDatabase || Environment.GetEnvironmentVariable("TF_BUILD") == null)
            {
                EnsureDatabase.For.SqlDatabase(connectionString);
            }
            var sw = Stopwatch.StartNew();

            var dbScriptVersioner = new DatabaseScriptVersioner(connectionString);
            if (dbScriptVersioner.IsDatabaseAtCurrentScriptVersion())
            {
                AnsiConsole.MarkupLine("[green]Database up to date - skipping migration[/]");
                return 0;
            }

            var migrationResult = RunDbActivity("Migration", () => Migrator.Migrate(connectionString));
            AnsiConsole.MarkupLine($"[green]Migration took {sw.Elapsed.TotalSeconds} seconds[/]");
            sw.Restart();

            var seedResult = settings.SeedData ? RunDbActivity("Seed Data Creation", () => Seeder.Seed(connectionString)) : null;
            AnsiConsole.MarkupLine($"[green]Seed Data Creation took {sw.Elapsed.TotalSeconds} seconds[/]");
            sw.Restart();

            var schemaResult = RunDbActivity("Views, TVFs and Stored Procedures", () => SchemaMigrator.Migrate(connectionString));
            AnsiConsole.MarkupLine($"[green]Views, TVFs and Stored Procedures took {sw.Elapsed.TotalSeconds} seconds[/]");
            sw.Restart();

            var mockResult = settings.MockData ? RunDbActivity("Mock Data Creation", () => Mocker.CreateMockData(connectionString)) : null;
            AnsiConsole.MarkupLine($"[green]Mock Data Creation took {sw.Elapsed.TotalSeconds} seconds[/]");
            sw.Restart();

            if (migrationResult.Successful
                && (!settings.SeedData || seedResult is { Successful: true})
                && schemaResult.Successful
                && (!settings.MockData || mockResult is { Successful: true }))
            {
                AnsiConsole.MarkupLine("[green]Success![/]");
                dbScriptVersioner.UpdateScriptVersion();
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
            throw;
        }

        return 0;
    }


    private static DatabaseUpgradeResult RunDbActivity(string activityName, Func<DatabaseUpgradeResult> action)
    {
        var result = action();

        if (result.Successful)
        {
            AnsiConsole.MarkupLine("[green]Complete[/]");
        }
        else
        {
            throw result.Error;
        }
        return result;
    }

}
