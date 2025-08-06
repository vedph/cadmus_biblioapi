using Microsoft.EntityFrameworkCore;
using System;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// EF data context for bibliography database.
/// </summary>
/// <seealso cref="DbContext" />
public sealed class BiblioDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the work types.
    /// </summary>
    public DbSet<EfWorkType> WorkTypes { get; set; }

    /// <summary>
    /// Gets or sets the authors.
    /// </summary>
    public DbSet<EfAuthor> Authors { get; set; }

    /// <summary>
    /// Gets or sets the author-work links.
    /// </summary>
    public DbSet<EfAuthorWork> AuthorWorks { get; set; }

    /// <summary>
    /// Gets or sets the author-container links.
    /// </summary>
    public DbSet<EfAuthorContainer> AuthorContainers { get; set; }

    /// <summary>
    /// Gets or sets the works.
    /// </summary>
    public DbSet<EfWork> Works { get; set; }

    /// <summary>
    /// Gets or sets the containers.
    /// </summary>
    public DbSet<EfContainer> Containers { get; set; }

    /// <summary>
    /// Gets or sets the keywords.
    /// </summary>
    public DbSet<EfKeyword> Keywords { get; set; }

    /// <summary>
    /// Gets or sets the keyword-work links.
    /// </summary>
    public DbSet<EfKeywordWork> KeywordWorks { get; set; }

    /// <summary>
    /// Gets or sets the keyword-container links.
    /// </summary>
    public DbSet<EfKeywordContainer> KeywordContainers { get; set; }

    /// <summary>
    /// Gets or sets the container links.
    /// </summary>
    public DbSet<EfContainerLink> ContainerLinks { get; set; }

    /// <summary>
    /// Gets or sets the work external links.
    /// </summary>
    public DbSet<EfWorkLink> WorkLinks { get; set; }

    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext

    /// <summary>
    /// Initializes a new instance of the <see cref="BiblioDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public BiblioDbContext(DbContextOptions<BiblioDbContext> options) :
        base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Biblio.Ef.BiblioContext" />
    /// class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="databaseType">Type of the database.</param>
    public BiblioDbContext(string connectionString, string databaseType) :
        base(GetOptions(connectionString, databaseType))
    {
    }

    /// <summary>
    /// Gets the options wrapping the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="databaseType">Type of the database.</param>
    /// <returns>Options.</returns>
    public static DbContextOptions<BiblioDbContext> GetOptions(
        string connectionString, string databaseType)
    {
        return (databaseType?.ToLowerInvariant()) switch
        {
            //_ => new DbContextOptionsBuilder<BiblioDbContext>()
            //  .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            //  .Options,
            _ => new DbContextOptionsBuilder<BiblioDbContext>()
              .UseNpgsql(connectionString)
              .Options,
        };
    }

    /// <summary>
    /// <para>
    /// Override this method to configure the database (and other options)
    /// to be used for this context.
    /// This method is called for each instance of the context that is
    /// created.
    /// </para>
    /// <para>
    /// In situations where an instance of <see cref="DbContextOptions" />
    /// may or may not have been passed to the constructor, you can use
    /// <see cref="DbContextOptionsBuilder.IsConfigured" /> to determine if
    /// the options have already been set, and skip some or all of the logic
    /// in <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)" />.
    /// </para>
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify
    /// options for this context. Databases (and other extensions)
    /// typically define extension methods on this object that allow you
    /// to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // note that these are fake credentials for development.
            // in production they would be replaced with environment data,
            // but in any case in production we would not hit this code
            //const string cs = "Server=localhost;Database=cadmus-biblio;Uid=root;Pwd=mysql;";
            //optionsBuilder.UseMySql(cs, ServerVersion.AutoDetect(cs));
            const string cs = "Server=localhost;port=5432;" +
                "Database=cadmus-biblio;User Id=postgres;Password=postgres;" +
                "Include Error Detail=True";
            optionsBuilder.UseNpgsql(cs);
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<DateTimeToUtcConverter>();
    }

    /// <summary>
    /// Override this method to further configure the model that was
    /// discovered by convention from the entity types exposed in
    /// <see cref="DbSet`1" /> properties on your derived context.
    /// The resulting model may be cached and re-used for subsequent
    /// instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct
    /// the model for this context. Databases (and other extensions)
    /// typically define extension methods on this object that allow you
    /// to configure aspects of the model that are specific to a given
    /// database.</param>
    /// <remarks>
    /// If a model is explicitly set on the options for this context
    /// (via <see cref="DbContextOptionsBuilder.UseModel(IModel)" />)
    /// then this method will not be run.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // work_type
        modelBuilder.Entity<EfWorkType>().ToTable("work_type");
        modelBuilder.Entity<EfWorkType>().Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(20);
        modelBuilder.Entity<EfWorkType>().Property(t => t.Name)
            .HasColumnName("name")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(100);

        // author
        modelBuilder.Entity<EfAuthor>().ToTable("author");
        modelBuilder.Entity<EfAuthor>().Property(a => a.Id)
            .HasColumnName("id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfAuthor>().Property(a => a.First)
            .HasColumnName("first")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfAuthor>().Property(a => a.Last)
            .HasColumnName("last")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfAuthor>().Property(a => a.Lastx)
            .HasColumnName("lastx")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfAuthor>().Property(a => a.Suffix)
            .HasColumnName("suffix")
            .IsUnicode()
            .HasMaxLength(50);

        // work
        modelBuilder.Entity<EfWork>().ToTable("work");
        modelBuilder.Entity<EfWork>().Property(w => w.Id)
            .HasColumnName("id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfWork>().Property(w => w.Key)
            .HasColumnName("key")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(300);
        modelBuilder.Entity<EfWork>().Property(w => w.TypeId)
            .HasColumnName("type_id")
            .IsUnicode(false)
            .HasMaxLength(20);
        modelBuilder.Entity<EfWork>().Property(w => w.ContainerId)
            .HasColumnName("container_id")
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfWork>().Property(w => w.Title)
            .HasColumnName("title")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(200);
        modelBuilder.Entity<EfWork>().Property(w => w.Titlex)
            .HasColumnName("titlex")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(200);
        modelBuilder.Entity<EfWork>().Property(w => w.Language)
            .HasColumnName("language")
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<EfWork>().Property(w => w.Edition)
            .HasColumnName("edition")
            .IsRequired();
        modelBuilder.Entity<EfWork>().Property(w => w.Publisher)
            .HasColumnName("publisher")
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfWork>().Property(w => w.YearPub)
            .HasColumnName("year_pub")
            .IsRequired();
        modelBuilder.Entity<EfWork>().Property(w => w.YearPub2)
            .HasColumnName("year_pub2");
        modelBuilder.Entity<EfWork>().Property(w => w.PlacePub)
            .HasColumnName("place_pub")
            .IsUnicode()
            .HasMaxLength(100);
        modelBuilder.Entity<EfWork>().Property(w => w.Location)
            .HasColumnName("location")
            .IsUnicode()
            .HasMaxLength(500);
        modelBuilder.Entity<EfWork>().Property(w => w.AccessDate)
            .HasColumnName("access_date");
        modelBuilder.Entity<EfWork>().Property(w => w.Number)
            .HasColumnName("number")
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfWork>().Property(w => w.FirstPage)
            .HasColumnName("first_page")
            .IsRequired();
        modelBuilder.Entity<EfWork>().Property(w => w.LastPage)
            .HasColumnName("last_page")
            .IsRequired();
        modelBuilder.Entity<EfWork>().Property(w => w.Note)
            .HasColumnName("note")
            .IsUnicode()
            .HasMaxLength(500);
        modelBuilder.Entity<EfWork>().Property(w => w.Datation)
            .HasColumnName("datation")
            .IsUnicode()
            .HasMaxLength(1000);
        modelBuilder.Entity<EfWork>().Property(w => w.DatationValue)
            .HasColumnName("datation_value");

        // work link
        modelBuilder.Entity<EfWorkLink>().ToTable("work_link");
        modelBuilder.Entity<EfWorkLink>().Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<EfWorkLink>().Property(l => l.SourceId)
            .HasColumnName("work_id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfWorkLink>().Property(l => l.Scope)
            .HasColumnName("scope")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(50);
        modelBuilder.Entity<EfWorkLink>().Property(l => l.Value)
            .HasColumnName("value")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(1000);

        // work has many links
        modelBuilder.Entity<EfWork>()
            .HasMany(w => w.Links)
            .WithOne(l => l.Source)
            .HasForeignKey(l => l.SourceId);

        // container
        modelBuilder.Entity<EfContainer>().ToTable("container");
        modelBuilder.Entity<EfContainer>().Property(w => w.Id)
            .HasColumnName("id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfContainer>().Property(w => w.Key)
            .HasColumnName("key")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(300);
        modelBuilder.Entity<EfContainer>().Property(w => w.TypeId)
            .HasColumnName("type_id")
            .IsUnicode(false)
            .HasMaxLength(20);
        modelBuilder.Entity<EfContainer>().Property(w => w.Title)
            .HasColumnName("title")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(200);
        modelBuilder.Entity<EfContainer>().Property(w => w.Titlex)
            .HasColumnName("titlex")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(200);
        modelBuilder.Entity<EfContainer>().Property(w => w.Language)
            .HasColumnName("language")
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<EfContainer>().Property(w => w.Edition)
            .HasColumnName("edition")
            .IsRequired();
        modelBuilder.Entity<EfContainer>().Property(w => w.Publisher)
            .HasColumnName("publisher")
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfContainer>().Property(w => w.YearPub)
            .HasColumnName("year_pub")
            .IsRequired();
        modelBuilder.Entity<EfContainer>().Property(w => w.YearPub2)
            .HasColumnName("year_pub2");
        modelBuilder.Entity<EfContainer>().Property(w => w.PlacePub)
            .HasColumnName("place_pub")
            .IsUnicode()
            .HasMaxLength(100);
        modelBuilder.Entity<EfContainer>().Property(w => w.Location)
            .HasColumnName("location")
            .IsUnicode()
            .HasMaxLength(500);
        modelBuilder.Entity<EfContainer>().Property(w => w.AccessDate)
            .HasColumnName("access_date");
        modelBuilder.Entity<EfContainer>().Property(w => w.Number)
            .HasColumnName("number")
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfContainer>().Property(w => w.Note)
            .HasColumnName("note")
            .IsUnicode()
            .HasMaxLength(500);
        modelBuilder.Entity<EfContainer>().Property(w => w.Datation)
            .HasColumnName("datation")
            .IsUnicode()
            .HasMaxLength(1000);
        modelBuilder.Entity<EfContainer>().Property(w => w.DatationValue)
            .HasColumnName("datation_value");

        // container link
        modelBuilder.Entity<EfContainerLink>().ToTable("container_link");
        modelBuilder.Entity<EfContainerLink>().Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<EfContainerLink>().Property(l => l.SourceId)
            .HasColumnName("container_id")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(36)
            .IsFixedLength();
        modelBuilder.Entity<EfContainerLink>().Property(l => l.Scope)
            .HasColumnName("scope")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(50);
        modelBuilder.Entity<EfContainerLink>().Property(l => l.Value)
            .HasColumnName("value")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(1000);

        // container has many links
        modelBuilder.Entity<EfContainer>()
            .HasMany(w => w.Links)
            .WithOne(l => l.Source)
            .HasForeignKey(l => l.SourceId);

        // keyword
        modelBuilder.Entity<EfKeyword>().ToTable("keyword");
        modelBuilder.Entity<EfKeyword>().Property(k => k.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<EfKeyword>().Property(k => k.Language)
            .HasColumnName("language")
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<EfKeyword>().Property(k => k.Value)
            .HasColumnName("value")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);
        modelBuilder.Entity<EfKeyword>().Property(k => k.Valuex)
            .HasColumnName("valuex")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);

        // https://dev.to/_patrickgod/many-to-many-relationship-with-entity-framework-core-4059
        // author_work
        modelBuilder.Entity<EfAuthorWork>().ToTable("author_work");
        modelBuilder.Entity<EfAuthorWork>()
            .HasKey(aw => new { aw.AuthorId, aw.WorkId });
        modelBuilder.Entity<EfAuthorWork>().Property(x => x.AuthorId)
            .HasColumnName("author_id")
            .HasMaxLength(36)
            .IsRequired();
        modelBuilder.Entity<EfAuthorWork>().Property(x => x.WorkId)
            .HasColumnName("work_id")
            .HasMaxLength(36)
            .IsRequired();
        modelBuilder.Entity<EfAuthorWork>().Property(x => x.Role)
            .HasColumnName("role")
            .HasMaxLength(50)
            .IsUnicode();
        modelBuilder.Entity<EfAuthorWork>().Property(x => x.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();

        // author_container
        modelBuilder.Entity<EfAuthorContainer>().ToTable("author_container");
        modelBuilder.Entity<EfAuthorContainer>()
            .HasKey(ac => new { ac.AuthorId, ac.ContainerId });
        modelBuilder.Entity<EfAuthorContainer>().Property(x => x.AuthorId)
            .HasColumnName("author_id")
            .HasMaxLength(36)
            .IsRequired();
        modelBuilder.Entity<EfAuthorContainer>().Property(x => x.ContainerId)
            .HasColumnName("container_id")
            .HasMaxLength(36)
            .IsRequired();
        modelBuilder.Entity<EfAuthorContainer>().Property(x => x.Role)
            .HasColumnName("role")
            .HasMaxLength(50)
            .IsUnicode();
        modelBuilder.Entity<EfAuthorContainer>().Property(x => x.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();

        // keyword_work
        modelBuilder.Entity<EfKeywordWork>().ToTable("keyword_work");
        modelBuilder.Entity<EfKeywordWork>()
            .HasKey(kw => new { kw.KeywordId, kw.WorkId });
        modelBuilder.Entity<EfKeywordWork>().Property(x => x.KeywordId)
            .HasColumnName("keyword_id")
            .IsRequired();
        modelBuilder.Entity<EfKeywordWork>().Property(x => x.WorkId)
            .HasColumnName("work_id")
            .HasMaxLength(36)
            .IsRequired();

        // keyword_container
        modelBuilder.Entity<EfKeywordContainer>().ToTable("keyword_container");
        modelBuilder.Entity<EfKeywordContainer>()
            .HasKey(kc => new { kc.KeywordId, kc.ContainerId });
        modelBuilder.Entity<EfKeywordContainer>().Property(x => x.KeywordId)
            .HasColumnName("keyword_id")
            .IsRequired();
        modelBuilder.Entity<EfKeywordContainer>().Property(x => x.ContainerId)
            .HasColumnName("container_id")
            .HasMaxLength(36)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
