using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Api.Extensions;

public class PostgresSqlConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PostgresSqlConfiguration(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void Configure()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    context.Database.OpenConnection();
                    context.Database.CloseConnection();

                    context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS unaccent;");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"Account\" SET \"CurrentClassId\" = (SELECT \"ClassId\" FROM \"StudentClass\" WHERE \"StudentClass\".\"StudentFirebaseId\" = \"Account\".\"AccountFirebaseId\" LIMIT 1) WHERE \"Role\" = 1"
                    );

                    break; // If connection is successful, break loop
                }
                catch (Exception ex)
                {
                    retryCount--;
                    Console.WriteLine($"Database connection failed. Retrying... ({retryCount} attempts left)");
                    Thread.Sleep(3000); // Wait 3 seconds before retrying
                }
            }
        }
    }

}