namespace Cadmus.Biblio.Cli.Services;

/// <summary>
/// CLI context service.
/// </summary>
public class BiblioCliContextService
{
    public BiblioCliContextServiceConfig Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiblioCliContextService"/>
    /// class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public BiblioCliContextService(BiblioCliContextServiceConfig config)
    {
        Configuration = config;
    }
}

/// <summary>
/// Configuration for <see cref="BiblioCliContextService"/>.
/// </summary>
public class BiblioCliContextServiceConfig
{
    /// <summary>
    /// Gets or sets the connection string to the database.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the local directory to use when loading resources
    /// from the local file system.
    /// </summary>
    public string? LocalDirectory { get; set; }
}
