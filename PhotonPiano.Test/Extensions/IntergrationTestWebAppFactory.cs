using DotNet.Testcontainers.Builders;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.Api;
using PhotonPiano.DataAccess.Models;
using Testcontainers.PostgreSql;

namespace PhotonPiano.Test.Extensions;

public class IntergrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("photonpiano")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await WaitForDatabaseReadiness();
    }

    private async Task WaitForDatabaseReadiness()
    {
        var connectionString = _dbContainer.GetConnectionString();
        var maxRetries = 10;
        var retryDelay = TimeSpan.FromSeconds(2);

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                using var connection = new Npgsql.NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                return;
            }
            catch (Exception)
            {
                if (i == maxRetries - 1)
                    throw;
                await Task.Delay(retryDelay);
            }
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        // Properly dispose Hangfire servers first
        try
        {
            using var scope = Services.CreateScope();
            var backgroundServer = scope.ServiceProvider.GetService<IBackgroundProcessingServer>();
            (backgroundServer as IDisposable)?.Dispose();
        }
        catch (Exception)
        {
            // Ignore disposal errors
        }

        // Stop the container after Hangfire has been disposed
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing DB context configuration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor != null) 
                services.Remove(dbContextDescriptor);

            // Remove existing Hangfire configurations
            var hangfireStorage = services.SingleOrDefault(d => d.ServiceType == typeof(JobStorage));
            if (hangfireStorage != null) 
                services.Remove(hangfireStorage);

            var hangfireServer = services.SingleOrDefault(d => d.ServiceType == typeof(IBackgroundProcessingServer));
            if (hangfireServer != null) 
                services.Remove(hangfireServer);

            var connectionString = _dbContainer.GetConnectionString();

            // Configure EF Core
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                });
            });

            // Configure Hangfire with complete settings
            services.AddHangfire((_, config) =>
            {
                config.UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                });
            });

            // Use minimal server configuration
            services.AddHangfireServer(options => {
                options.WorkerCount = 1;
                options.ShutdownTimeout = TimeSpan.FromSeconds(5);
                options.ServerName = $"test-server-{Guid.NewGuid()}";
                options.Queues = new[] { "default" };
                options.StopTimeout = TimeSpan.FromSeconds(3);
            });
        });

        base.ConfigureWebHost(builder);
    }
}