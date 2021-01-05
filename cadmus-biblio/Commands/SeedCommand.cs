using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using Cadmus.Biblio.Seed;
using Fusi.DbManager.MySql;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Cadmus.Biblio.Commands
{
    public sealed class SeedCommand : ICommand
    {
        private readonly IConfiguration _config;
        private readonly string _database;
        private readonly int _count;
        private readonly string _entities;

        public SeedCommand(AppOptions options, string database, int count,
            string entities)
        {
            _config = options.Configuration;
            _database = database ??
                throw new ArgumentNullException(nameof(database));
            _count = count;
            _entities = entities?.ToUpperInvariant();
        }

        public static void Configure(CommandLineApplication command,
            AppOptions options)
        {
            command.Description = "Create and seed a Cadmus bibliographic database";
            command.HelpOption("-?|-h|--help");

            CommandArgument databaseArgument = command.Argument("[database]",
                "The database name");

            CommandOption countOption = command.Option("-c|--count",
                "Items count (default=100)",
                CommandOptionType.SingleValue);

            CommandOption partsOption = command.Option("-e|--entities",
                "The list of entities to be seeded: " +
                "T(ypes) A(uthors) C(containers) K(eywords) W(orks)",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                int count = 100;
                if (countOption.HasValue())
                {
                    int.TryParse(countOption.Value(), out count);
                }

                options.Command = new SeedCommand(options,
                    databaseArgument.Value,
                    count,
                    partsOption.HasValue()? partsOption.Value() : null);
                return 0;
            });
        }

        public Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SEED DATABASE\n");
            Console.ResetColor();
            Console.WriteLine(
                $"Input: {_database}\n" +
                $"Count: {_count}\n" +
                $"Entities: {_entities ?? "all"}");

            // create database if not exists
            string connection = string.Format(CultureInfo.InvariantCulture,
                _config.GetConnectionString("Default"),
                _database);

            MySqlDbManager manager = new MySqlDbManager(connection);

            if (!manager.Exists(_database))
            {
                Console.WriteLine("Creating database...");
                Serilog.Log.Information($"Creating database {_database}...");

                manager.CreateDatabase(_database, EfHelper.GetSchema(), null);

                Console.WriteLine("Database created.");
                Serilog.Log.Information("Database created.");
            }

            Console.Write("Seeding...");
            IBiblioRepository repository =
                new EfBiblioRepository(connection, "mysql");
            BiblioSeeder seeder = new BiblioSeeder(repository);

            seeder.Seed(_count, _entities);

            Console.WriteLine(" completed");

            return Task.CompletedTask;
        }
    }
}
