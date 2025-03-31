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
        .WithDatabase("photonpiano")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(false)
         .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();


        var maxRetries = 10;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                using var conn = new NpgsqlConnection(GetDbConnectionString());
                await conn.OpenAsync();
                return;
            }
            catch
            {
                await Task.Delay(1000);
            }
        }

        throw new Exception("PostgreSQL container did not start in time.");
    }


    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }


    public string GetDbConnectionString()
    {
        return _dbContainer.GetConnectionString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null) services.Remove(descriptor);


            var connectionString = _dbContainer.GetConnectionString();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

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
}