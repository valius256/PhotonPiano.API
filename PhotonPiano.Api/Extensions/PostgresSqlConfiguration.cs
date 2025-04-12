using PhotonPiano.DataAccess.Models;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.Api.Extensions;

public class PostgresSqlConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PostgresSqlConfiguration(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private string GetLevelUpdateQuery(Guid levelId, decimal theoryScore, decimal practicalScore)
    {
        return $"UPDATE public.\"Level\" " +
               $"SET \"MinimumPracticalScore\" = {practicalScore}, \"MinimumTheoreticalScore\" = {theoryScore} " +
               $"WHERE \"Id\" = '{levelId}'";
    }

    private string GetAccountUpdateQuery()
    {
        var hashedPassword = AuthUtils.HashPassword("123456");

        return $"UPDATE public.\"Account\" " +
               $"SET \"Password\" = '{hashedPassword}'";
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
                        "UPDATE \"Account\" " +
                        "SET \"CurrentClassId\" = (" +
                        "    SELECT \"sc\".\"ClassId\" " +
                        "    FROM \"StudentClass\" AS \"sc\" " +
                        "    JOIN \"Class\" AS \"c\" ON \"sc\".\"ClassId\" = \"c\".\"Id\" " +
                        "    WHERE \"sc\".\"StudentFirebaseId\" = \"Account\".\"AccountFirebaseId\" " +
                        "    AND (\"c\".\"Status\" = 0 OR \"c\".\"Status\" = 1) " +
                        "    AND \"c\".\"RecordStatus\" = 1 " +
                        "    LIMIT 1" +
                        ") " +
                        "WHERE \"Role\" = 1"
                    );

                    // do here to update level 

                    context.Database.ExecuteSqlRaw(GetLevelUpdateQuery(Guid.Parse("8fb54d6e-315c-470d-825e-91b8314134f7"), 0.0m, 0.0m));
                    context.Database.ExecuteSqlRaw(GetLevelUpdateQuery(Guid.Parse("ad04326c-4d91-4d67-bc76-2c1dfb87c2c5"), 2.0m, 2.0m));
                    context.Database.ExecuteSqlRaw(GetLevelUpdateQuery(Guid.Parse("3db94220-15db-4880-9b57-b6064b27c11b"), 4.0m, 4.0m));
                    context.Database.ExecuteSqlRaw(GetLevelUpdateQuery(Guid.Parse("0d232654-2ce8-4193-9bc3-acd3eddf2ff2"), 6.0m, 6.0m));
                    context.Database.ExecuteSqlRaw(GetLevelUpdateQuery(Guid.Parse("55974743-7c93-47ab-877e-eda4cb9f96c5"), 8.0m, 8.0m));
                    
                    context.Database.ExecuteSqlRaw(GetAccountUpdateQuery());

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