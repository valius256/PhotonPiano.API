using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ConfigureDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await EnsureDatabaseCreateAsync(dbContext);
            await EnsureDatabaseMigrateAsync(dbContext);
        }


        private static async Task EnsureDatabaseCreateAsync(ApplicationDbContext dbContext)
        {
            try
            {
                var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    if (!await dbCreator.ExistsAsync())
                    {
                        await dbCreator.CreateAsync();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task EnsureDatabaseMigrateAsync(ApplicationDbContext dbContext)
        {
            try
            {
                var strategy = dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await dbContext.Database.MigrateAsync();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
