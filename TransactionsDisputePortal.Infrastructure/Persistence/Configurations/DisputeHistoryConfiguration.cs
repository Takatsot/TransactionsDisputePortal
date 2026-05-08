using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for DisputeHistory entity
    /// </summary>
    public class DisputeHistoryConfiguration : IEntityTypeConfiguration<DisputeHistory>
    {
        public void Configure(EntityTypeBuilder<DisputeHistory> builder)
        {
            builder.ToTable("DisputeHistory");

            builder.HasKey(h => h.Id);

            // Base entity properties
            builder.Property(h => h.Id)
                .IsRequired();

            builder.Property(h => h.CreatedDate)
                .IsRequired();

            builder.Property(h => h.UpdatedDate)
                .IsRequired(false);

            builder.Property(h => h.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(h => h.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(256);

            // DisputeHistory specific properties
            builder.Property(h => h.DisputeId)
                .IsRequired();

            builder.Property(h => h.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(h => h.Notes)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(h => h.ChangedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(h => h.ChangedDate)
                .IsRequired();

            // Indexes
            builder.HasIndex(h => h.DisputeId)
                .HasDatabaseName("IX_DisputeHistory_DisputeId");

            builder.HasIndex(h => h.ChangedDate)
                .HasDatabaseName("IX_DisputeHistory_ChangedDate");

            // Relationships
            builder.HasOne(h => h.Dispute)
                .WithMany(d => d.History)
                .HasForeignKey(h => h.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
