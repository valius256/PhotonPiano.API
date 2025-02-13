using System.Reflection;
using Hangfire;
using Mapster;
using OfficeOpenXml;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Extensions;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.DataAccess.Extensions;
using PhotonPiano.PubSub;
using PhotonPiano.ServiceDefaults;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<DbMigrationJob>();
var configuration = builder.Configuration;

// if (configuration.GetValue<bool  >("IsAspireHost")) builder.AddRedisClient("redis-cache");

// EPPlus Configuration
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddApiDependencies(configuration)
    .AddBusinessLogicDependencies()
    .AddDataAccessDependencies();

builder.AddPubSub();

builder.Services.AddSingleton<RedirectUrlValidator>();

//Add serilog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
builder.AddServiceDefaults();
// add mapster 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

var app = builder.Build();
app.UseRouting();
app.MapDefaultEndpoints();

await app.ConfigureDatabaseAsync();

app.UseScalarConfig();

app.UseCors("AllowAll");

// Register and execute PostgresSqlConfiguration
var sqlConfig = app.Services.GetRequiredService<PostgresSqlConfiguration>();
sqlConfig.Configure(app, app.Environment);

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "PhotonPiano Dashboard",
    DarkModeEnabled = true,
    IsReadOnlyFunc = _ => false,
    TimeZoneResolver = new DefaultTimeZoneResolver()
});


app.UseExceptionHandler();

app.UseAuthorization();

app.MapPubSub();

app.MapControllers();

await app.RunAsync();

//This Startup endpoint for Unit Tests
namespace PhotonPiano.Api
{
    public class Program;
}