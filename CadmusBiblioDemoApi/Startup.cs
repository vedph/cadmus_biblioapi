﻿using System.Text;
using System.Text.Json;
using MessagingApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AspNetCore.Identity.Mongo;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using System.Reflection;
using Cadmus.Core;
using Cadmus.Seed;
using Cadmus.Core.Config;
using Cadmus.Index.Config;
using Cadmus.Api.Services.Auth;
using Cadmus.Api.Services.Messaging;
using Cadmus.Api.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Cadmus.BiblioDemo.Services;
using Cadmus.Core.Storage;
using Cadmus.Export.Preview;

namespace CadmusBiblioDemoApi;

/// <summary>
/// Startup.
/// </summary>
public sealed class Startup
{
    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the host environment.
    /// </summary>
    public IHostEnvironment HostEnvironment { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environment.</param>
    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        HostEnvironment = environment;
    }

    private void ConfigureOptionsServices(IServiceCollection services)
    {
        // configuration sections
        // https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/
        services.Configure<MessagingOptions>(Configuration.GetSection("Messaging"));
        services.Configure<DotNetMailerOptions>(Configuration.GetSection("Mailer"));

        // explicitly register the settings object by delegating to the IOptions object
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<MessagingOptions>>().Value);
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<DotNetMailerOptions>>().Value);
    }

    private void ConfigureCorsServices(IServiceCollection services)
    {
        string[] origins = new[] { "http://localhost:4200" };

        IConfigurationSection section = Configuration.GetSection("AllowedOrigins");
        if (section.Exists())
        {
            origins = section.AsEnumerable()
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => p.Value).ToArray()!;
        }

        services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                // https://github.com/aspnet/SignalR/issues/2110 for AllowCredentials
                .AllowCredentials()
                .WithOrigins(origins);
        }));
    }

    private void ConfigureAuthServices(IServiceCollection services)
    {
        // identity
        string connStringTemplate = Configuration.GetConnectionString("Default")!;

        services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(
            options => { },
            mongoOptions =>
            {
                mongoOptions.ConnectionString =
                    string.Format(connStringTemplate,
                    Configuration.GetSection("DatabaseNames")["Auth"]);
            });

        // authentication service
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // NOTE: remember to set the values in configuration:
                // Jwt:SecureKey, Jwt:Audience, Jwt:Issuer
                IConfigurationSection jwtSection = Configuration.GetSection("Jwt");
                string key = jwtSection["SecureKey"]!;
                if (string.IsNullOrEmpty(key))
                    throw new InvalidOperationException("Required JWT SecureKey not found");

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidIssuer = jwtSection["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
#if DEBUG
        // use to show more information when troubleshooting JWT issues
        IdentityModelEventSource.ShowPII = true;
#endif
    }

    private static void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API",
                Description = "Cadmus BiblioDemo Services"
            });
            c.DescribeAllParametersInCamelCase();

            // include XML comments
            // (remember to check the build XML comments in the prj props)
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);

            // JWT
            // https://stackoverflow.com/questions/58179180/jwt-authentication-and-swagger-with-net-core-3-0
            // (cf. https://ppolyzos.com/2017/10/30/add-jwt-bearer-authorization-to-swagger-and-asp-net-core/)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
        });
        });
    }

    private CadmusPreviewer GetPreviewer(IServiceProvider provider)
    {
        // get dependencies
        ICadmusRepository repository =
                provider.GetService<IRepositoryProvider>()!.CreateRepository();
        ICadmusPreviewFactoryProvider factoryProvider =
            new StandardCadmusPreviewFactoryProvider();

        // nope if disabled
        if (!Configuration.GetSection("Preview").GetSection("IsEnabled")
            .Get<bool>())
        {
            return new CadmusPreviewer(factoryProvider.GetFactory("{}"),
                repository);
        }

        // get profile source
        ILogger<Startup>? logger = provider.GetService<ILogger<Startup>>();
        IHostEnvironment env = provider.GetService<IHostEnvironment>()!;
        string path = Path.Combine(env.ContentRootPath,
            "wwwroot", "preview-profile.json");
        if (!File.Exists(path))
        {
            Console.WriteLine($"Preview profile expected at {path} not found");
            logger?.LogError("Preview profile expected at {path} not found", path);
            return new CadmusPreviewer(factoryProvider.GetFactory("{}"),
                repository);
        }

        // load profile
        Console.WriteLine($"Loading preview profile from {path}...");
        logger?.LogInformation("Loading preview profile from {path}...", path);
        string profile;
        using (StreamReader reader = new(new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
        {
            profile = reader.ReadToEnd();
        }
        CadmusPreviewFactory factory = factoryProvider.GetFactory(profile);

        return new CadmusPreviewer(factory, repository);
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // configuration
        ConfigureOptionsServices(services);

        // CORS (before MVC)
        ConfigureCorsServices(services);

        // base services
        services.AddControllers();
        // camel-case JSON in response
        services.AddMvc()
            // https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-2.2&tabs=visual-studio#jsonnet-support
            // Newtonsoft is required by MongoDB
            .AddNewtonsoftJson()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase;
            });

        // authentication
        ConfigureAuthServices(services);

        // Add framework services
        // for IMemoryCache: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory
        services.AddMemoryCache();

        // user repository service
        services.AddTransient<IUserRepository<ApplicationUser>,
            ApplicationUserRepository>();

        // messaging
        // TODO: you can use another mailer service here. In this case,
        // also change the types in ConfigureOptionsServices.
        services.AddTransient<IMailerService, DotNetMailerService>();
        services.AddTransient<IMessageBuilderService,
            FileMessageBuilderService>();

        // configuration
        services.AddSingleton(_ => Configuration);
        // repository
        string dataCS = string.Format(
            Configuration.GetConnectionString("Default")!,
            Configuration.GetValue<string>("DatabaseNames:Data"));
        services.AddSingleton<IRepositoryProvider>(
          _ => new BiblioDemoRepositoryProvider { ConnectionString = dataCS });

        // part seeder factory provider
        services.AddSingleton<IPartSeederFactoryProvider,
            BiblioDemoPartSeederFactoryProvider>();
        // item browser factory provider
        services.AddSingleton<IItemBrowserFactoryProvider>(_ =>
            new StandardItemBrowserFactoryProvider(
                Configuration.GetConnectionString("Default")!));
        // item index factory provider
        string indexCS = string.Format(
            Configuration.GetConnectionString("Index")!,
            Configuration.GetValue<string>("DatabaseNames:Data"));
        services.AddSingleton<IItemIndexFactoryProvider>(_ =>
            new StandardItemIndexFactoryProvider(indexCS));

        // previewer
        services.AddSingleton(p => GetPreviewer(p));

        // swagger
        ConfigureSwaggerServices(services);
    }

    /// <summary>
    /// Configures the specified application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2#configure-a-reverse-proxy-server
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor
                | ForwardedHeaders.XForwardedProto
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio
            app.UseExceptionHandler("/Error");
            if (Configuration.GetValue<bool>("Server:UseHSTS"))
            {
                Console.WriteLine("HSTS: yes");
                app.UseHsts();
            }
            else Console.WriteLine("HSTS: no");
        }

        if (Configuration.GetValue<bool>("Server:UseHttpsRedirection"))
        {
            Console.WriteLine("HttpsRedirection: yes");
            app.UseHttpsRedirection();
        }
        else Console.WriteLine("HttpsRedirection: no");

        app.UseHttpsRedirection();
        app.UseRouting();
        // CORS
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            string? url = Configuration.GetValue<string>("Swagger:Endpoint");
            if (string.IsNullOrEmpty(url)) url = "v1/swagger.json";
            options.SwaggerEndpoint(url, "V1 Docs");
        });
    }
}
