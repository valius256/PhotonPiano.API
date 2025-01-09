using Mapster;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Extensions;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.DataAccess.Extensions;
using Serilog;
using StackExchange.Redis;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<DbMigrationJob>();


var configuration = builder.Configuration;

if (configuration.GetValue<bool>("EnableMigration") == true)
{
    builder.AddRedisClient("redis-cache");
}
else
{
    var redisConnectionString = configuration.GetSection("Redis")["ConnectionString"];
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisConnectionString));

}



// Add services to the container.
builder.Services.AddApiDependencies(configuration)
    .AddBusinessLogicDependencies()
    .AddDataAccessDependencies();

//builder.Services.AddRateLimiter(options =>
//{
//    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

//    options.AddFixedWindowLimiter("fixed-window", opt =>
//    {
//        opt.Window = TimeSpan.FromSeconds(10);
//        opt.PermitLimit = 3;
//        opt.QueueLimit = 0;
//        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.NewestFirst;
//    });

//});


builder.Services.AddTransient<PostgresSqlConfiguration>();
//Add serilog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

// add mapster 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapDefaultEndpoints();

await app.ConfigureDatabaseAsync();


app.UseScalarConfig();

app.UseCors("AllowAll");

// Register and execute PostgresSqlConfiguration
var sqlConfig = app.Services.GetRequiredService<PostgresSqlConfiguration>();
sqlConfig.Configure(app, app.Environment);

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();


//This Startup endpoint for Unit Tests
namespace PhotonPiano.Api { public class Program; }