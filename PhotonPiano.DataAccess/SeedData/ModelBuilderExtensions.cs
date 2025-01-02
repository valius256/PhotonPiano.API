using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.SeedData
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>().HasData(
                new Account { Id = Guid.NewGuid(), Name = "John Doe", Age = 30 },
                new Account { Id = Guid.NewGuid(), Name = "Jane Doe", Age = 25 }
            );

        }
    }
}
