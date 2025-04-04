using System.Net;
using Hangfire;
using Mapster;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Extensions;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.DataAccess.Extensions;
using PhotonPiano.PubSub;
using Serilog;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
            var passphrase = Environment.GetEnvironmentVariable("CERT_PEM_PASSPHRASE");

            // Load the certificate with the passphrase
            var cert = new X509Certificate2("/etc/ssl/certs/combined-certificate.pem", passphrase);

            // If you have a separate private key file, you can load it like this:
            // var key = File.ReadAllText("/etc/ssl/private/mydomain.key");
            // var cert = new X509Certificate2(certPath, passphrase, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            listenOptions.UseHttps(cert);
        });
    });
}




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