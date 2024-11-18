using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using Cadmus.Biblio.Seed;
using Fusi.DbManager.PgSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CadmusBiblioApi.Services;

/// <summary>
/// IHost bibliographic database seeder extensions.
/// </summary>
public static class BiblioHostSeedExtensions
{
    private static Task SeedBiblioAsync(IServiceProvider serviceProvider)
    {
        return Policy.Handle<DbException>()
            .WaitAndRetry(
            [
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60)
            ], (exception, timeSpan, _) =>
            {
                ILogger logger = serviceProvider
                    .GetService<ILoggerFactory>()!
                    .CreateLogger(typeof(BiblioHostSeedExtensions));

                string message = "Unable to connect to DB" +
                    $" (sleep {timeSpan}): {exception.Message}";
                Console.WriteLine(message);
                logger.LogError(exception, message);
            }).Execute(() =>
            {
                IConfiguration config = serviceProvider.GetService<IConfiguration>()!;

                ILogger? logger = serviceProvider
                    .GetService<ILoggerFactory>()!
                    .CreateLogger(typeof(BiblioHostSeedExtensions));

                Console.WriteLine("Seeding database...");
                IBiblioRepository repository =
                    serviceProvider.GetService<IBiblioRepository>()!;

                BiblioSeeder seeder = new(repository)
                {
                    Logger = logger
                };
                seeder.Seed(config.GetValue<int>("Seed:EntityCount"));

                Console.WriteLine("Seeding completed");
                return Task.CompletedTask;
            });
    }

    private static async Task CreateBiblioAsync(IServiceProvider serviceProvider)
    {
        await Policy.Handle<DbException>()
            .WaitAndRetry(
            [
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60)
            ], (exception, timeSpan, _) =>
            {
                // in case of DbException we must retry
                ILogger? logger = serviceProvider
                    .GetService<ILoggerFactory>()!
                    .CreateLogger(typeof(BiblioHostSeedExtensions));

                string message = "Unable to connect to DB" +
                    $" (sleep {timeSpan}): {exception.Message}";
                Console.WriteLine(message);
                logger.LogError(exception, message);
            }).Execute(async () =>
            {
                IConfiguration config =
                    serviceProvider.GetService<IConfiguration>()!;

                // delay if requested, to allow DB start
                int delay = config.GetValue<int>("Seed:BiblioDelay");
                if (delay > 0)
                {
                    Console.WriteLine($"Waiting for {delay} seconds...");
                    Thread.Sleep(delay * 1000);
                }
                else Console.WriteLine("No delay for seeding");

                // if the DB does not exist, create and seed it
                string dbName = config.GetValue<string>("DatabaseNames:Biblio")!;
                string cst = config.GetConnectionString("Biblio")!;
                Console.WriteLine($"Checking for database {dbName}...");
                Serilog.Log.Information("Checking for database {Name}...", dbName);

                PgSqlDbManager manager = new(cst);
                if (!manager.Exists(dbName))
                {
                    // create
                    Console.WriteLine("Creating database...");
                    Serilog.Log.Information("Creating database {Name}...", dbName);

                    manager.CreateDatabase(dbName, EfHelper.GetSchema(), null);

                    Console.WriteLine("Database created.");
                    Serilog.Log.Information("Database created.");

                    // seed
                    Console.WriteLine("Seeding database...");
                    Serilog.Log.Information("Seeding database...");

                    await SeedBiblioAsync(serviceProvider);

                    Console.WriteLine("Seeding completed.");
                    Serilog.Log.Information("Seeding completed.");
                }
            });
    }

    /// <summary>
    /// Seeds the database.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns>The received host, to allow concatenation.</returns>
    /// <exception cref="ArgumentNullException">serviceProvider</exception>
    public static async Task<IHost> SeedBiblioAsync(this IHost host)
    {
        Console.WriteLine("Seeding biblio...");

        using (var scope = host.Services.CreateScope())
        {
            IServiceProvider serviceProvider = scope.ServiceProvider;
            ILogger? logger = serviceProvider
                .GetService<ILoggerFactory>()!
                .CreateLogger(typeof(BiblioHostSeedExtensions));

            try
            {
                await CreateBiblioAsync(serviceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
        return host;
    }
}
