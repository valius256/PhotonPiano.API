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

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps("/root/server.crt", "/root/server.key");
    });
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