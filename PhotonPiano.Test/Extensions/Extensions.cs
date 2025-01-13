using Microsoft.Extensions.Hosting;
using PhotonPiano.DataAccess.Models;

namespace PhotonPiano.Test.Extensions;

internal static class Extensions
{
    public static async Task EnsureDbCreated(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}