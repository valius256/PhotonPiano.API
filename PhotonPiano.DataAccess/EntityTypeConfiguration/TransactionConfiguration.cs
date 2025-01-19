using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedTransaction)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Tution)
            .WithMany(x => x.TransactionTutions)
            .HasForeignKey(x => x.TutionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.EntranceTestStudent)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.EntranceTestStudentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}