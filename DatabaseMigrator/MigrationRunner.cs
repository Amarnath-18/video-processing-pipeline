
using DbUp;
using Microsoft.Extensions.Logging;

namespace DotnetPractice.DatabaseMigrator
{
    public static class MigrationRunner
    {
        public static void Run(string connectionString, ILogger logger)
        {
            logger.LogInformation("Running database migration with connection string: {ConnectionString}", connectionString);
            var scriptsPath = Path.Combine(AppContext.BaseDirectory, "Scripts");

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem(scriptsPath)
                .LogToConsole()
                .JournalToPostgresqlTable("public", "schemaversions")
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                logger.LogError(result.Error, "Migration failed");
                throw result.Error;
            }

            logger.LogInformation("datbase migration completed successfully");

        }
    }
}