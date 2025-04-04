using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Test.Extensions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (_dbContext.Database.GetPendingMigrations().Any()) _dbContext.Database.Migrate();
    }
}