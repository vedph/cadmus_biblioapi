﻿using Cadmus.Biblio.Cli.Commands;
using Cadmus.Biblio.Cli.Services;
using Fusi.Cli.Commands;
using Microsoft.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cadmus.Biblio.Cli;

public static class Program
{
#if DEBUG
    private static void DeleteLogs()
    {
        foreach (var path in Directory.EnumerateFiles(
            AppDomain.CurrentDomain.BaseDirectory, "cadmus-biblio-log*.txt"))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
#endif

    private static BiblioCliAppContext? GetAppContext(string[] args)
    {
        return new CliAppContextBuilder<BiblioCliAppContext>(args)
            .SetNames("Biblio", "Biblio CLI")
            .SetLogger(new SerilogLoggerProvider(Log.Logger)
                .CreateLogger(nameof(Program)))
            .SetDefaultConfiguration()
            .SetCommands(new Dictionary<string,
                Action<CommandLineApplication, ICliAppContext>>
            {
                ["seed"] = SeedCommand.Configure,
            })
        .Build();
    }

    public static int Main(string[] args)
    {
        try
        {
            // https://github.com/serilog/serilog-sinks-file
            string logFilePath = Path.Combine(
                Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) ?? "",
                    "Biblio-log.txt");
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
#if DEBUG
            DeleteLogs();
#endif
            Console.OutputEncoding = Encoding.UTF8;
            Stopwatch stopwatch = new();
            stopwatch.Start();

            Task.Run(async () =>
            {
                BiblioCliAppContext? context = GetAppContext(args);

                if (context?.Command == null)
                {
                    // RootCommand will have printed help
                    return 1;
                }

                Console.Clear();
                await context.Command.Run();
                return 0;
            }).Wait();

            Console.ResetColor();
            Console.CursorVisible = true;
            Console.WriteLine();
            Console.WriteLine();

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Console.WriteLine("\nTime: {0}h{1}'{2}\"",
                    stopwatch.Elapsed.Hours,
                    stopwatch.Elapsed.Minutes,
                    stopwatch.Elapsed.Seconds);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.ToString());
            Console.ResetColor();
            return 2;
        }
    }
}
