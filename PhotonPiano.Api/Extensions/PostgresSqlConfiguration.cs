using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Api.Extensions;

public class PostgresSqlConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PostgresSqlConfiguration(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Execute SQL after DbContext is fully configured
            context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS unaccent;");
        }
    }
}