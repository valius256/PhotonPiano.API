using DotNet.Testcontainers.Builders;
using Hangfire;
using Hangfire.PostgreSql;
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
        .WithPortBinding(5432, true) // Use random port to avoid conflicts
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
                
                // Test query to verify database is fully ready
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                
                // If we get here, the database is ready
                return;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                    throw new Exception($"Database failed to become available after {maxRetries} attempts", ex);
                
                await Task.Delay(retryDelay);
            }
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null) services.Remove(descriptor);

            var connectionString = _dbContainer.GetConnectionString();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });
            });

            // Configure Hangfire for testing
            services.AddHangfire((_, config) =>
            {
                config.UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                });
                
                // Disable dashboard and server in test environment
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            });
            
            services.AddHangfireServer(options => {
               options.WorkerCount = 15;
               options.Queues = new[] { "critical", "default" };
            });
            
        });

        base.ConfigureWebHost(builder);
    }
}