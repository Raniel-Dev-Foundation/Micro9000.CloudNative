using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using System.Reflection;

namespace DatabaseManagement;

internal static class Mocker
{
    private const string mockFolderNamespace = "DatabaseManagement.Mock";

    public static DatabaseUpgradeResult CreateMockData(string connectionString)
    {
        var runtimeEnvironment = Environment.GetEnvironmentVariable("ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(runtimeEnvironment))
        {
            // If no environment is set then return successful noop
            Console.WriteLine("No environment detected for mock data.");
            return new DatabaseUpgradeResult(Enumerable.Empty<SqlScript>(), true, null, null);
        }

        var environments = new List<string> { "Common" }; // Always run Common mock scripts
        if (!environments.Any(e => e.ToLowerInvariant().Equals(runtimeEnvironment.ToLowerInvariant())))
        {
            // Apply mock overrides per environment
            environments.Add(runtimeEnvironment);
        }

        Console.WriteLine($"Executing mock scrpts for '{runtimeEnvironment}' environment");

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (s => ShouldRunScript(s, environments)))
            .JournalTo(new NullJournal())
            .LogToConsole()
            .Build();
        return upgrader.PerformUpgrade();
    }

    private static bool ShouldRunScript(string script, IEnumerable<string> environments)
    {
        var mockFolders = environments.Select(e => $"{mockFolderNamespace}.{e}");
        return mockFolders.Any(f => script.StartsWith(f, StringComparison.OrdinalIgnoreCase));
    }
}
