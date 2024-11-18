using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Cadmus.Api.Services;
using Cadmus.Api.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Cadmus.Api.Config.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;
using Cadmus.Biblio.Api.Controllers;
using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using System.Globalization;
using CadmusBiblioApi.Services;

namespace CadmusBiblioApi;

/// <summary>
/// Main program.
/// </summary>
public static class Program
{
    private static void ConfigureAppServices(IServiceCollection services,
        IConfiguration config)
    {
        services.AddTransient<IBiblioRepository>(_ =>
        {
            string cs = string.Format(
                CultureInfo.InvariantCulture,
                config.GetConnectionString("Biblio")!,
                config.GetValue<string>("DatabaseNames:Biblio"));

            return new EfBiblioRepository(cs, "pgsql");
        });
    }

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
            ServiceConfigurator.DumpEnvironmentVars();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ServiceConfigurator.ConfigureLogger(builder);
            IConfiguration config = new ConfigurationService(builder.Environment)
                .Configuration;
            ServiceConfigurator.ConfigureServices(builder.Services, config,
                builder.Environment);
            ConfigureAppServices(builder.Services, config);

            builder.Services.AddOpenApi();

            // controllers from Cadmus.Api.Controllers
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(AuthorController).Assembly)
                .AddControllersAsServices();

            WebApplication app = builder.Build();

            // forward headers for use with an eventual reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor
                    | ForwardedHeaders.XForwardedProto
            });

            // development or production
            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio
                app.UseExceptionHandler("/Error");
                if (config.GetValue<bool>("Server:UseHSTS"))
                {
                    Console.WriteLine("HSTS: yes");
                    app.UseHsts();
                }
                else
                {
                    Console.WriteLine("HSTS: no");
                }
            }

            // HTTPS redirection
            if (config.GetValue<bool>("Server:UseHttpsRedirection"))
            {
                Console.WriteLine("HttpsRedirection: yes");
                app.UseHttpsRedirection();
            }
            else
            {
                Console.WriteLine("HttpsRedirection: no");
            }

            // CORS
            app.UseCors("CorsPolicy");
            // rate limiter
            if (!config.GetValue<bool>("RateLimit:IsDisabled"))
                app.UseRateLimiter();
            // authentication
            app.UseAuthentication();
            app.UseAuthorization();
            // proxy
            app.UseResponseCaching();

            // seed auth database (via Services/HostAuthSeedExtensions)
            await app.SeedAuthAsync();

            // seed biblio database (via Services/HostSeedExtension)
            await app.SeedBiblioAsync();

            // map controllers and Scalar API
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();

            Log.Information("Running API");
            await app.RunAsync();

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
            await Log.CloseAndFlushAsync();
        }
    }
}
