using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using Cadmus.Biblio.Seed;
using Cadmus.Index.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;

namespace Cadmus.Biblio.Api.Services
{
    /// <summary>
    /// IHost bibliographic database seeder extensions.
    /// </summary>
    public static class BiblioHostSeedExtensions
    {
        private static Task SeedBiblioAsync(IServiceProvider serviceProvider)
        {
            return Policy.Handle<DbException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(60)
                }, (exception, timeSpan, _) =>
                {
                    ILogger logger = serviceProvider
                        .GetService<ILoggerFactory>()
                        .CreateLogger(typeof(BiblioHostSeedExtensions));

                    string message = "Unable to connect to DB" +
                        $" (sleep {timeSpan}): {exception.Message}";
                    Console.WriteLine(message);
                    logger.LogError(exception, message);
                }).Execute(() =>
                {
                    IConfiguration config =
                        serviceProvider.GetService<IConfiguration>();

                    ILogger logger = serviceProvider
                        .GetService<ILoggerFactory>()
                        .CreateLogger(typeof(BiblioHostSeedExtensions));

                    Console.WriteLine("Seeding database...");
                    IBiblioRepository repository =
                        serviceProvider.GetService<IBiblioRepository>();

                    BiblioSeeder seeder = new BiblioSeeder(repository)
                    {
                        Logger = logger
                    };
                    seeder.Seed(config.GetValue<int>("Seed:ItemCount"));

                    Console.WriteLine("Seeding completed");
                    return Task.CompletedTask;
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
            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                ILogger logger = serviceProvider
                    .GetService<ILoggerFactory>()
                    .CreateLogger(typeof(BiblioHostSeedExtensions));

                try
                {
                    // get DB connection string template and name from config
                    IConfiguration config =
                        serviceProvider.GetService<IConfiguration>();

                    string dbName = config.GetValue<string>("DatabaseNames:Biblio");
                    string cs = string.Format(
                        CultureInfo.InvariantCulture,
                        config.GetConnectionString("Biblio"),
                        dbName);

                    // if the DB does not exist, create and seed it
                    MySqlDbManager manager = new MySqlDbManager(cs);

                    if (!manager.Exists(dbName))
                    {
                        // create
                        Console.WriteLine("Creating database...");
                        Serilog.Log.Information($"Creating database {dbName}...");

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
}
