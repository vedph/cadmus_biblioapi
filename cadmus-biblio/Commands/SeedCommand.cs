using Cadmus.Biblio.Cli.Services;
using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using Cadmus.Biblio.Seed;
using Fusi.Cli.Commands;
using Fusi.DbManager.MySql;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Cadmus.Biblio.Cli.Commands;

internal sealed class SeedCommand : ICommand
{
    private readonly SeedCommandOptions _options;

    private SeedCommand(SeedCommandOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public static void Configure(CommandLineApplication app,
        ICliAppContext context)
    {
        app.Description = "Create and seed a Cadmus bibliographic database";
        app.HelpOption("-?|-h|--help");

        CommandArgument databaseArgument = app.Argument("[database]",
            "The database name");

        CommandOption countOption = app.Option("-c|--count",
            "Items count (default=100)",
            CommandOptionType.SingleValue);

        CommandOption partsOption = app.Option("-e|--entities",
            "The list of entities to be seeded: " +
            "T(ypes) A(uthors) C(containers) K(eywords) W(orks)",
            CommandOptionType.SingleValue);

        app.OnExecute(() =>
        {
            context.Command = new SeedCommand(new SeedCommandOptions(context)
            {
                Database = databaseArgument.Value,
                Count = countOption.HasValue() &&
                    int.TryParse(countOption.Value(), out int n) ? n : 100,
                Entities = partsOption.HasValue() ? partsOption.Value() : null
            });

            return 0;
        });
    }

    public Task<int> Run()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("SEED DATABASE\n");
        Console.ResetColor();
        Console.WriteLine(
            $"Database: {_options.Database}\n" +
            $"Count: {_options.Count}\n" +
            $"Entities: {_options.Entities ?? "all"}");

        // create database if not exists
        string connection = string.Format(CultureInfo.InvariantCulture,
            _options.Context.Configuration!.GetConnectionString("Default")!,
            _options.Database);

        MySqlDbManager manager = new(connection);

        if (!manager.Exists(_options.Database))
        {
            Console.WriteLine("Creating database...");
            Serilog.Log.Information($"Creating database {_options.Database}...");

            manager.CreateDatabase(_options.Database, EfHelper.GetSchema(), null);

            Console.WriteLine("Database created.");
            Serilog.Log.Information("Database created.");
        }

        Console.Write("Seeding...");
        IBiblioRepository repository = new EfBiblioRepository(connection, "mysql");
        BiblioSeeder seeder = new(repository);

        seeder.Seed(_options.Count, _options.Entities);

        Console.WriteLine(" completed");

        return Task.FromResult(0);
    }
}

internal class SeedCommandOptions : CommandOptions<BiblioCliAppContext>
{
    public string Database { get; set; }
    public int Count { get; set; }

    /// <summary>
    /// The optional selector for the entities to be seeded; in this string
    /// T=types, K=keywords, A=authors, C=containers, W=works. When specified,
    /// only the entities listed in this string are seeded.
    /// </summary>
    public string? Entities { get; set; }

    public SeedCommandOptions(ICliAppContext context)
        : base((BiblioCliAppContext)context)
    {
        Database = "cadmus-biblio";
    }
}
