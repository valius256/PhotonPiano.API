using Hangfire;
using Hangfire.PostgreSql;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Middlewares;
using PhotonPiano.Api.Requests.Application;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.Survey;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BackgroundJob;
using PhotonPiano.BusinessLogic.BusinessModel.Application;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;
using StackExchange.Redis;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.Api.Extensions;

public static class IServiceCollectionExtensions
{
    private static bool _messagePrinted;

    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllerConfigurations()
            .AddAuthConfigurations(configuration)
            .AddExceptionHandlerConfiguration()
            .AddScalarConfigurations()
            .AddSettingsOptions(configuration)
            .AddDbContextConfigurations(configuration)
            .AddHangFireConfigurations(configuration)
            .AddCorsConfigurations()
            .AddMapsterConfig()
            .AddRedisCache(configuration)
            .AddPostGresSqlConfiguration()
            .AddRazorTemplateWithConfigPath()
            .AddRateLimitedForAllEndpoints()
            .ConfigureResponseCompression()
            .AddHealthChecks(configuration)
            ;


        return services;
    }

    private static IServiceCollection AddExceptionHandlerConfiguration(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    private static IServiceCollection AddAuthConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = configuration["Firebase:Auth:Authority"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Firebase:Auth:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Firebase:Auth:Audience"],
                ValidateLifetime = true
            };
            options.IncludeErrorDetails = true;
        });
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        TypeAdapterConfig<EntranceTestDetailModel, EntranceTestResponse>.NewConfig()
            .Map(dest => dest.RegisterStudents, src => src.EntranceTestStudents.Count)
            .Map(dest => dest.Status, src => src.RecordStatus);

        TypeAdapterConfig<EntranceTestWithInstructorModel, EntranceTestResponse>.NewConfig()
            .Map(dest => dest.Status, src => src.RecordStatus);

        //TypeAdapterConfig<SignUpRequest, SignUpModel>.NewConfig()
        //    .Map(dest => dest.DesiredLevel, src => src.Level);

        TypeAdapterConfig<AutoArrangeEntranceTestsRequest, AutoArrangeEntranceTestsModel>
            .NewConfig()
            .Map(dest => dest.ShiftOptions, src => src.ShiftOptions.OrderBy(s => (int)s));

        TypeAdapterConfig<UpdateApplicationRequest, UpdateApplicationModel>.NewConfig()
            .Map(dest => dest.StaffConfirmNote, src => src.Note);

        TypeAdapterConfig<UpdateApplicationModel, Application>.NewConfig().IgnoreNullValues(true);

        TypeAdapterConfig<EntranceTestDetailModel, EntranceTestDetailResponse>.NewConfig()
            .Map(dest => dest.RegisterStudents, src => src.EntranceTestStudents.Count)
            .Map(dest => dest.Status, src => src.RecordStatus);

        TypeAdapterConfig<UpdateEntranceTestResultsRequest, UpdateEntranceTestResultsModel>.NewConfig()
            .IgnoreNullValues(true);

        TypeAdapterConfig<StudentClassModel, StudentClass>.NewConfig()
            .Map(dest => dest.Student, src => (StudentClassModel?)null);

        TypeAdapterConfig<AutoArrangeEntranceTestsRequest, AutoArrangeEntranceTestsModel>.NewConfig()
            .Map(dest => dest.StartDate, src => DateTime.SpecifyKind(src.StartDate, DateTimeKind.Unspecified))
            .Map(dest => dest.EndDate, src => DateTime.SpecifyKind(src.EndDate, DateTimeKind.Unspecified));

        TypeAdapterConfig<CreatePianoSurveyRequest, CreatePianoSurveyModel>.NewConfig()
            .Map(dest => dest.CreateQuestionRequests, src => src.Questions);

        TypeAdapterConfig<UpdateCriteriaModel, Criteria>.NewConfig()
            .IgnoreNullValues(true);

        return services;
    }


    private static IServiceCollection AddSettingsOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Appsettings>(configuration.GetSection("Appsettings"));

        services.Configure<SmtpAppSetting>(configuration.GetSection("SmtpAppSetting"));
        services.Configure<FirebaseUpload>(configuration.GetSection("FirebaseUpload"));

        services.Configure<VnPay>(configuration.GetSection("VnPay"));

        services.Configure<PayOsOption>(configuration.GetSection("PayOsOption"));

        services.Configure<AllowedRedirectDomainsConfig>(
            configuration.GetSection("AllowedRedirectDomains"));

        return services;
    }

    private static IServiceCollection AddScalarConfigurations(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi(options => { options.AddDocumentTransformer<OpenApiSecuritySchemeTransformer>(); });
        services.AddEndpointsApiExplorer();
        return services;
    }

    private static IServiceCollection AddCorsConfigurations(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowAll", p => p
                .WithExposedHeaders("X-Total-Count", "X-Total-Pages", "X-Page", "X-Page-Size")
                .WithOrigins("http://localhost:5173","https://photon-piano.vercel.app", "https://photon-piano.netlify.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                // .AllowAnyOrigin()
                .AllowCredentials()
            // .SetIsOriginAllowed(_ => true)
            )
        );
        return services;
    }

    private static string? GetConnectionString(this IConfiguration configuration)
    {
        var envConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");
        var rs = !string.IsNullOrEmpty(envConnectionString)
            ? envConnectionString
            : configuration.GetValue<bool>("IsDeploy")
                ? configuration.GetConnectionString("PostgresDeploy")
                : configuration.GetConnectionString("PostgresPhotonPiano");

        if (!_messagePrinted && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            Console.WriteLine("This running is using connection string: " + rs);
            _messagePrinted = true;
        }

        return rs;
    }


    private static IServiceCollection AddDbContextConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration) +
                               ";Maximum Pool Size=40;Minimum Pool Size=5;Connection Lifetime=60;Pooling=true";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
            options.EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information);
            options.EnableDetailedErrors();
        });

        return services;
    }

    private static IServiceCollection AddControllerConfigurations(this IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {

        var redisConnectionString = configuration.GetSection("ConnectionStrings")["RedisConnectionStrings"];
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            redisConnectionString = "photonpiano-cache:6379";
        }
        if (!_messagePrinted && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            Console.WriteLine("This running is using redis connection string: " + redisConnectionString);
            _messagePrinted = true;
        }
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString!, options =>
            {
                options.ConnectRetry = 5;
                options.ConnectTimeout = 5000;
            }));

        return services;
    }

    private static IServiceCollection AddPostGresSqlConfiguration(this IServiceCollection services)
    {
        services.AddTransient<PostgresSqlConfiguration>();

        return services;
    }

    private static IServiceCollection AddRazorTemplateWithConfigPath(this IServiceCollection services)
    {
        services.AddRazorTemplating();

        return services;
    }


    private static IServiceCollection AddHangFireConfigurations(this IServiceCollection services, IConfiguration configuration)
{
    services.AddHangfire((_, config) =>
    {
        config.UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(GetConnectionString(configuration));
        });
    });

    var cultureInfo = new CultureInfo("vn-VN");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    services.AddTransient<IDefaultScheduleJob, DefaultScheduleJob>();
    
    services.AddHangfireServer((serviceProvider, cf) =>
    {
        cf.WorkerCount = 15;
        cf.TimeZoneResolver = new DefaultTimeZoneResolver();

        var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
        

        using (var scope = serviceProvider.CreateScope())
        {
            var configService = scope.ServiceProvider.GetRequiredService<ISystemConfigService>();

            var tuitionReminderDay = configService.GetConfig(ConfigNames.TuitionPaymentReminderDate).Result?.ConfigValue ?? "15";
            var tuitionOverdueDay = configService.GetConfig(ConfigNames.TuitionPaymentDeadline).Result?.ConfigValue ?? "28";
            
            recurringJobManager.AddOrUpdate<TuitionService>("AutoCreateTuitionInStartOfMonth",
                x => x.CronAutoCreateTuition(),
                Cron.Monthly);

            recurringJobManager.AddOrUpdate<TuitionService>("TuitionReminder",
                x => x.CronForTuitionReminder(),
                $"0 0 {tuitionReminderDay} * *");

            recurringJobManager.AddOrUpdate<SlotService>("AutoChangedSlotStatus",
                x => x.CronAutoChangeSlotStatus(),
                Cron.Hourly());

            recurringJobManager.AddOrUpdate<TuitionService>("TuitionOverdue",
                x => x.CronForTuitionOverdue(),
                $"0 0 {tuitionOverdueDay} * *");

            recurringJobManager.AddOrUpdate<NotificationService>("AutoRemovedOutDateNotifications",
                x => x.CronJobAutoRemovedOutDateNotifications(),
                Cron.Hourly(15));
        }
    });

    return services;
}


    private static IServiceCollection AddRateLimitedForAllEndpoints(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            // GlobalLimiter is used for all controllers
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress ?? IPAddress.Loopback; // Sử dụng localhost nếu null
                return RateLimitPartition.GetFixedWindowLimiter(ipAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100, // 100 requests per window
                        Window = TimeSpan.FromMinutes(1), // Per 1 minute window
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
            });
        });
        return services;
    }

    private static IServiceCollection ConfigureResponseCompression(this IServiceCollection services)
    {
        // Brotli and Gzip reduce the size of the outgoing JSON, HTML or Static files data.
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });

        services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });

        return services;
    }
    
    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            // .AddRedis(
            //     configuration.GetConnectionString("RedisConnectionStrings"),
            //     name: "redis",
            //     failureStatus: HealthStatus.Degraded,
            //     tags: new[] { "db", "redis" })
            .AddNpgSql(
                GetConnectionString(configuration)!,
                name: "postgres",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "postgresql" });

        return services;
    }
}