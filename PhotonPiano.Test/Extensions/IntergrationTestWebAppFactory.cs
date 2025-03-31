using DotNet.Testcontainers.Builders;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PhotonPiano.Api;
using PhotonPiano.DataAccess.Models;
using Testcontainers.PostgreSql;

namespace PhotonPiano.Test.Extensions;

public class IntergrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
             .WithPortBinding(5432, 0)  // Cổng tự động chọn nếu 5432 bị chiếm
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .WithDatabase("photonpiano")
        .WithUsername("postgres")
        .WithPassword("postgres")
         .WithAutoRemove(false)
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Xóa DbContext cũ nếu có
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbDescriptor is not null) services.Remove(dbDescriptor);

            var connectionString = _dbContainer.GetConnectionString();

            // Kiểm tra xem DB có sẵn chưa trước khi khởi động
            EnsureDatabaseReady(connectionString);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Xóa Hangfire cũ nếu có
            var hangfireDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBackgroundJobClient));
            if (hangfireDescriptor is not null) services.Remove(hangfireDescriptor);

            services.AddHangfire((_, config) =>
            {
                config.UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                });
            });
        });

        base.ConfigureWebHost(builder);
    }

    private void EnsureDatabaseReady(string connectionString)
    {
        var retry = 10;
        while (retry > 0)
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("✅ Database is ready!");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Database not ready yet. Retrying... {retry} attempts left. Error: {ex.Message}");
                Task.Delay(1000).Wait();
                retry--;
            }
        }

        throw new Exception("❌ Database is not ready for Hangfire.");
    }
}