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
        .WithImage("postgres:latest")
        .WithDatabase("photonpiano")
        .WithUsername("postgres")
        .WithPassword("anhphatdeptrai") 
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
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });

        base.ConfigureWebHost(builder);
    }
}