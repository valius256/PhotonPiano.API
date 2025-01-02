using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        
    }
}