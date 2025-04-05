using Hangfire;
using Mapster;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Extensions;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.DataAccess.Extensions;
using PhotonPiano.PubSub;
using Serilog;
using System.Reflection;
using System.Runtime.InteropServices;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc.Razor;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PhotonPiano.BusinessLogic.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<DbMigrationJob>();
var configuration = builder.Configuration;

// if (configuration.GetValue<bool  >("IsAspireHost")) builder.AddRedisClient("redis-cache");

// hello this line write to proved i am the owner of this project and github is not good for beginner



// Add services to the container.
builder.Services.AddApiDependencies(configuration)
    .AddBusinessLogicDependencies()
    .AddDataAccessDependencies();

if (OperatingSystem.IsLinux())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Any, 8080); // HTTP
        options.Listen(IPAddress.Any, 8081, listenOptions =>
        {
            listenOptions.UseHttps("/etc/ssl/certs/combined-certificate.pem");
        });
    });
}

// Load wkhtmltopdf native libraries
// Modify the path to point to wkhtmltox folder
var wkhtmltoxPath = Path.Combine(Directory.GetCurrentDirectory(), "wkhtmltox", "v0.12.4");
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(wkhtmltoxPath, 
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "libwkhtmltox.dll" : 
    RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "libwkhtmltox.so" : 
    "libwkhtmltox.dylib"));

// Add DinkToPdf services
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

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
// Not Done Yet
// builder.Services
//     .AddOpenTelemetry()
//     .ConfigureResource(resource => resource.AddService("PhotonPiano.Api"))
//     .WithTracing(tracerProviderBuilder =>
//     {
//         tracerProviderBuilder
//             .AddAspNetCoreInstrumentation() // Tracking API request
//             .AddHttpClientInstrumentation() // Tracking HTTP request
//             .AddSqlClientInstrumentation();  // Tracking database queries
//
//         
//         tracerProviderBuilder.AddOtlpExporter();
//     });


builder.AddSignalRConfig();

builder.Services.AddSingleton<RedirectUrlValidator>();

//Add serilog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
//builder.AddServiceDefaults();
// add mapster 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();
app.UseRouting();
//app.MapDefaultEndpoints();

await app.ConfigureDatabaseAsync();

app.UseScalarConfig();

app.UseCors("AllowAll");

// Register and execute PostgresSqlConfiguration
// var sqlConfig = app.Services.GetRequiredService<PostgresSqlConfiguration>();
// await sqlConfig.Configure();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "PhotonPiano Dashboard",
    DarkModeEnabled = true,
    IsReadOnlyFunc = _ => false,
    TimeZoneResolver = new DefaultTimeZoneResolver()
});

app.UseStaticFiles();

app.MapRazorPages();

var sqlConfig = app.Services.GetRequiredService<PostgresSqlConfiguration>();
sqlConfig.Configure();

// uncomment to active Rate limiter
// app.UseRateLimiter();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapSignalRConfig();

app.UseResponseCompression();

app.MapControllers();


app.MapHealthChecks("/health");


await app.RunAsync();

//This Startup endpoint for Unit Tests
namespace PhotonPiano.Api
{
    public class Program;
}