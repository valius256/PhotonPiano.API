using System.Reflection;
using DinkToPdf;
using DinkToPdf.Contracts;
using Ghostscript.NET.Rasterizer;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Mvc.Razor;
using OpenTelemetry.Metrics;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Extensions;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.DataAccess.Extensions;
using PhotonPiano.PubSub;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<DbMigrationJob>();
var configuration = builder.Configuration;

// if (configuration.GetValue<bool  >("IsAspireHost")) builder.AddRedisClient("redis-cache");

// hello this line write to proved i am the owner of this project and github is not good for beginner


// Add services to the container.
builder.Services.AddApiDependencies(configuration)
    .AddBusinessLogicDependencies()
    .AddDataAccessDependencies();


builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddPrometheusExporter();

        builder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel");
        builder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new[]
                {
                    0, 0.005, 0.01, 0.025, 0.05,
                    0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                }
            });
    });

// var wkhtmltoxPath = Path.Combine(Directory.GetCurrentDirectory(), "wkhtmltox", "v0.12.6");
// var context = new CustomAssemblyLoadContext();
// context.LoadUnmanagedLibrary(Path.Combine(wkhtmltoxPath,
//      RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "libwkhtmltox.dll" :
//      RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "libwkhtmltox.so" :
//      "libwkhtmltox.dylib"));

// Add DinkToPdf services
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Register Ghostscript.NET services
builder.Services.AddSingleton<GhostscriptRasterizer>(provider =>
{
    // You can optionally set a specific Ghostscript version path here if needed:
    // var gsPath = Path.Combine(Directory.GetCurrentDirectory(), "ghostscript", "gsdll32.dll");
    // GhostscriptVersionInfo gsVersion = new GhostscriptVersionInfo(gsPath);
    return new GhostscriptRasterizer();
});

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/Views/{0}" + RazorViewEngine.ViewExtension);
    options.ViewLocationFormats.Add("/Views/{0}.cshtml");
    options.ViewLocationFormats.Add("/{0}.cshtml");
    options.ViewLocationFormats.Add("/{0}");
});
//Not Done Yet
// builder.Services
//     .AddOpenTelemetry()
//     .ConfigureResource(resource => resource.AddService("PhotonPiano.Api"))
//     .WithTracing(tracerProviderBuilder =>
//     {
//         tracerProviderBuilder
//             .AddAspNetCoreInstrumentation() // Tracking API request
//             .AddHttpClientInstrumentation() // Tracking HTTP request
//             .AddSqlClientInstrumentation();  // Tracking database queries

//tracerProviderBuilder.AddOtlpExporter();
//     });


builder.AddSignalRConfig();

builder.Services.AddSingleton<RedirectUrlValidator>();


//Add serilog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
//builder.AddServiceDefaults();
// add mapster 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

//builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();


// app.UseMetricServer(); // expose Prometheus metrics at /metrics
// app.UseHttpMetrics(); // collect HTTP request metrics


app.UseRouting();


await app.ConfigureDatabaseAsync();

app.UseScalarConfig();

app.UseCors("AllowAll");

// app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "PhotonPiano Dashboard",
    DarkModeEnabled = true,
    IsReadOnlyFunc = _ => false,
    Authorization = new[] { new HangfireAuthorizationFilter() },
    AppPath = "https://photonpiano.duckdns.org/scalar/v1"
});

app.UseStaticFiles();
app.MapRazorPages();

// Register and execute PostgresSqlConfiguration
var sqlConfig = app.Services.GetRequiredService<PostgresSqlConfiguration>();
sqlConfig.Configure();

// uncomment to active Rate limiter
// app.UseRateLimiter();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapSignalRConfig();


app.UseResponseCompression();

app.MapControllers();

app.MapPrometheusScrapingEndpoint();

app.MapHealthChecks("/health");

await app.RunAsync();

//This Startup endpoint for Unit Tests
namespace PhotonPiano.Api
{
    public class Program;
}