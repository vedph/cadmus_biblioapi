using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Cadmus.Api.Services.Seeding;
using System.Threading.Tasks;
using Cadmus.Biblio.Api.Services;
using Microsoft.Extensions.Configuration;
using Cadmus.Api.Services;

namespace CadmusBiblioApi;

/// <summary>
/// Main program.
/// </summary>
public static class Program
{
    private static void DumpEnvironmentVars()
    {
        Console.WriteLine("ENVIRONMENT VARIABLES:");
        IDictionary dct = Environment.GetEnvironmentVariables();
        List<string> keys = new();
        var enumerator = dct.GetEnumerator();
        while (enumerator.MoveNext())
        {
            keys.Add(((DictionaryEntry)enumerator.Current).Key!.ToString()!);
        }

        foreach (string key in keys.OrderBy(s => s))
            Console.WriteLine($"{key} = {dct[key]}");
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    /// <summary>
    /// Entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>0=ok, else error.</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Log.Information("Starting biblio host");
            DumpEnvironmentVars();

            // this is the place for seeding:
            // see https://stackoverflow.com/questions/45148389/how-to-seed-in-entity-framework-core-2
            // and https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/?view=aspnetcore-2.1#move-database-initialization-code
            var host = await CreateHostBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    string cs = hostingContext.Configuration
                        .GetConnectionString("Log")!;
                    var maxSize = hostingContext.Configuration["Serilog:MaxMbSize"];

                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
#if DEBUG
                        .WriteTo.File("cadmus-biblio-log.txt",
                            rollingInterval: RollingInterval.Day)
#endif
                        .WriteTo.MongoDBCapped(cs,
                            cappedMaxSizeMb: !string.IsNullOrEmpty(maxSize) &&
                                int.TryParse(maxSize, out int n) && n > 0 ? n : 10);
                })
                .Build()
                // Cadmus seeder (authentication)
                .SeedAsync(accounts: true, data: false);

            // Biblio seeder
            await host.SeedBiblioAsync();

            host.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Biblio host terminated unexpectedly");
            Debug.WriteLine(ex.ToString());
            Console.WriteLine(ex.ToString());
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
