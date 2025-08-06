using Fusi.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;

namespace Cadmus.Biblio.Cli.Services;

/// <summary>
/// CLI app context.
/// </summary>
/// <seealso cref="CliAppContext" />
public class BiblioCliAppContext : CliAppContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiblioCliAppContext"/>
    /// class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public BiblioCliAppContext(IConfiguration? config, ILogger? logger)
        : base(config, logger)
    {
    }

    /// <summary>
    /// Gets the context service.
    /// </summary>
    /// <param name="dbName">The database name.</param>
    /// <exception cref="ArgumentNullException">dbName</exception>
    public virtual BiblioCliContextService GetContextService(string dbName)
    {
        ArgumentNullException.ThrowIfNull(dbName);

        return new BiblioCliContextService(
            new BiblioCliContextServiceConfig
            {
                ConnectionString = string.Format(CultureInfo.InvariantCulture,
                    Configuration!.GetConnectionString("Default")!, dbName),
                LocalDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets")
            });
    }
}
