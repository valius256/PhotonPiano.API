using Hangfire;
using Hangfire.PostgreSql;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotonPiano.Api.Configurations;
using PhotonPiano.Api.Middlewares;
using PhotonPiano.Api.Requests.Auth;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BackgroundJob;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Models;
using StackExchange.Redis;

namespace PhotonPiano.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllerConfigurations()
            .AddAuthConfigurations(configuration)
            .AddExceptionHandlerConfiguration()
            .AddScalarConfigurations()
            .AddSettingsOptions(configuration)
            .AddDbContextConfigurations(configuration)
            .AddFireBaseServices(configuration)
            .AddHangFireConfigurations(configuration)
            .AddCorsConfigurations()
            .AddMapsterConfig()
            .AddRedisCache(configuration)
            .AddPostGresSqlConfiguration()
            .AddRazorTemplateWithConfigPath()
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

        TypeAdapterConfig<SignUpRequest, SignUpModel>.NewConfig()
            .Map(dest => dest.DesiredLevel, src => src.Level);

        TypeAdapterConfig<AutoArrangeEntranceTestsRequest, AutoArrangeEntranceTestsModel>
            .NewConfig()
            .Map(dest => dest.ShiftOptions, src => src.ShiftOptions.OrderBy(s => (int)s));

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
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            )
        );
        return services;
    }

    
    
    private static bool MessagePrinted = false;
    
    private static string? GetConnectionString(this IConfiguration configuration)
    {
        var rs = configuration.GetValue<bool>("IsDeploy")
            ? configuration.GetConnectionString("PostgresDeployDb")
            : configuration.GetConnectionString("PostgresLocal");

        if (configuration.GetValue<bool>("IsAspireHost"))
            rs = configuration.GetConnectionString("photonpiano");

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && !MessagePrinted)
        {
            Console.WriteLine("This running is using connection string: " + rs);
            MessagePrinted = true;
        }

        return rs;
    }


    private static IServiceCollection AddDbContextConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(GetConnectionString(configuration));
            options.EnableSensitiveDataLogging();
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

    public static IServiceCollection AddFireBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        //var firebaseSettings = configuration.GetSection(nameof(Appsettings.FireBase)).Get<FireBase>();
        //var firebaseJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "fit-swipe-161d7-firebase-adminsdk-l0tth-9884dc9fa1.json");
        //FirebaseApp.Create(new AppOptions
        //{
        //    Credential = GoogleCredential.FromFile(firebaseJsonPath),
        //    ProjectId = firebaseSettings?.ProjectId,
        //});

        ////var storageClient = StorageClient.Create(GoogleCredential.FromFile(firebaseJsonPath));


        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("IsCacheDeploy"))
        {
            var redisConnectionString = configuration.GetSection("ConnectionStrings")["RedisConnectionStrings"];
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString!, options =>
                {
                    options.ConnectRetry = 5;
                    options.ConnectTimeout = 5000;
                }));
        }

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


    private static IServiceCollection AddHangFireConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire((_, config) =>
        {
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(GetConnectionString(configuration));
            });
        });

        services.AddHangfireServer();
        // Register Hangfire and configure it
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(GetConnectionString(configuration),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    PrepareSchemaIfNecessary = true
                })
        );

        // Register Hangfire server
        services.AddHangfireServer();

        // Register any other required services here
        services.AddTransient<IDefaultScheduleJob, DefaultScheduleJob>();

        services.AddHangfireServer(_ =>
        {
            RecurringJob.AddOrUpdate<TutionService>(x =>
                x.CronAutoCreateTution(), Cron.Monthly());
        });


        return services;
    }
}