using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotnetPractice.DatabaseMigrator;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var configuration = host.Services.GetRequiredService<IConfiguration>();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    logger.LogError("Connection string 'DefaultConnection' not found in configuration");
    Environment.Exit(1);
}

try
{
    logger.LogInformation("Starting database migration..");
    MigrationRunner.Run(connectionString, logger);
    logger.LogInformation("Migration completed successfully");
}catch(Exception ex)
{
    logger.LogError($"Failed to run {ex}");
}
