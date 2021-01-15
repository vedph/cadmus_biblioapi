using Microsoft.EntityFrameworkCore;

namespace Cadmus.Biblio.Ef
{
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
            switch (databaseType?.ToLowerInvariant())
            {
                default:
                    return new DbContextOptionsBuilder<BiblioDbContext>()
                        .UseMySql(connectionString)
                        .Options;
                    //return new DbContextOptionsBuilder<BiblioDbContext>()
                    //    .UseSqlServer(connectionString)
                    //    .Options;
            }
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
                optionsBuilder.UseMySql(
                    "Server=localhost;Database=cadmus-biblio;Uid=root;Pwd=mysql;");
            }
            base.OnConfiguring(optionsBuilder);
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
            // worktype
            modelBuilder.Entity<EfWorkType>().ToTable("worktype");
            modelBuilder.Entity<EfWorkType>().Property(t => t.Id)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(20);
            modelBuilder.Entity<EfWorkType>().Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(100);

            // author
            modelBuilder.Entity<EfAuthor>().ToTable("author");
            modelBuilder.Entity<EfAuthor>().Property(a => a.Id)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(36)
                .IsFixedLength();
            modelBuilder.Entity<EfAuthor>().Property(a => a.First)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfAuthor>().Property(a => a.Last)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfAuthor>().Property(a => a.Lastx)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfAuthor>().Property(a => a.Suffix)
                .IsUnicode()
                .HasMaxLength(50);

            // work
            modelBuilder.Entity<EfWork>().ToTable("work");
            modelBuilder.Entity<EfWork>().Property(w => w.Id)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(36)
                .IsFixedLength();
            modelBuilder.Entity<EfWork>().Property(w => w.Key)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(300);
            modelBuilder.Entity<EfWork>().Property(w => w.TypeId)
                .IsUnicode(false)
                .HasMaxLength(20);
            modelBuilder.Entity<EfWork>().Property(w => w.ContainerId)
                .HasMaxLength(36)
                .IsFixedLength();
            modelBuilder.Entity<EfWork>().Property(w => w.Title)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(200);
            modelBuilder.Entity<EfWork>().Property(w => w.Titlex)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(200);
            modelBuilder.Entity<EfWork>().Property(w => w.Language)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(3)
                .IsFixedLength();
            modelBuilder.Entity<EfWork>().Property(w => w.Edition)
                .IsRequired();
            modelBuilder.Entity<EfWork>().Property(w => w.Publisher)
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfWork>().Property(w => w.YearPub)
                .IsRequired();
            modelBuilder.Entity<EfWork>().Property(w => w.PlacePub)
                .IsUnicode()
                .HasMaxLength(100);
            modelBuilder.Entity<EfWork>().Property(w => w.Location)
                .IsUnicode()
                .HasMaxLength(500);
            modelBuilder.Entity<EfWork>().Property(w => w.Number)
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfWork>().Property(w => w.FirstPage)
                .IsRequired();
            modelBuilder.Entity<EfWork>().Property(w => w.LastPage)
                .IsRequired();
            modelBuilder.Entity<EfWork>().Property(w => w.Note)
                .IsUnicode()
                .HasMaxLength(500);

            // container
            modelBuilder.Entity<EfContainer>().ToTable("container");
            modelBuilder.Entity<EfContainer>().Property(w => w.Id)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(36)
                .IsFixedLength();
            modelBuilder.Entity<EfContainer>().Property(w => w.Key)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(300);
            modelBuilder.Entity<EfWork>().Property(w => w.TypeId)
                .IsUnicode(false)
                .HasMaxLength(20);
            modelBuilder.Entity<EfContainer>().Property(w => w.Title)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(200);
            modelBuilder.Entity<EfContainer>().Property(w => w.Titlex)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(200);
            modelBuilder.Entity<EfContainer>().Property(w => w.Language)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(3)
                .IsFixedLength();
            modelBuilder.Entity<EfContainer>().Property(w => w.Edition)
                .IsRequired();
            modelBuilder.Entity<EfContainer>().Property(w => w.Publisher)
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfContainer>().Property(w => w.YearPub)
                .IsRequired();
            modelBuilder.Entity<EfContainer>().Property(w => w.PlacePub)
                .IsUnicode()
                .HasMaxLength(100);
            modelBuilder.Entity<EfContainer>().Property(w => w.Location)
                .IsUnicode()
                .HasMaxLength(500);
            modelBuilder.Entity<EfContainer>().Property(w => w.Number)
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfContainer>().Property(w => w.Note)
                .IsUnicode()
                .HasMaxLength(500);

            // keyword
            modelBuilder.Entity<EfKeyword>().ToTable("keyword");
            modelBuilder.Entity<EfKeyword>().Property(k => k.Id)
                .IsRequired()
                .UseMySqlIdentityColumn();
            modelBuilder.Entity<EfKeyword>().Property(k => k.Language)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(3)
                .IsFixedLength();
            modelBuilder.Entity<EfKeyword>().Property(k => k.Value)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);
            modelBuilder.Entity<EfKeyword>().Property(k => k.Valuex)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            // https://dev.to/_patrickgod/many-to-many-relationship-with-entity-framework-core-4059
            // authorwork
            modelBuilder.Entity<EfAuthorWork>().ToTable("authorwork");
            modelBuilder.Entity<EfAuthorWork>()
                .HasKey(aw => new { aw.AuthorId, aw.WorkId });

            // authorcontainer
            modelBuilder.Entity<EfAuthorContainer>().ToTable("authorcontainer");
            modelBuilder.Entity<EfAuthorContainer>()
                .HasKey(ac => new { ac.AuthorId, ac.ContainerId });

            // keywordwork
            modelBuilder.Entity<EfKeywordWork>().ToTable("keywordwork");
            modelBuilder.Entity<EfKeywordWork>()
                .HasKey(kw => new { kw.KeywordId, kw.WorkId });

            // keywordcontainer
            modelBuilder.Entity<EfKeywordContainer>().ToTable("keywordcontainer");
            modelBuilder.Entity<EfKeywordContainer>()
                .HasKey(kc => new { kc.KeywordId, kc.ContainerId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
