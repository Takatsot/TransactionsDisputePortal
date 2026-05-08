using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    public class DisputeAttachmentConfiguration : IEntityTypeConfiguration<DisputeAttachment>
    {
        public void Configure(EntityTypeBuilder<DisputeAttachment> builder)
        {
            builder.ToTable("DisputeAttachments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisputeId)
                .IsRequired();

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.Property(x => x.StoragePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.UpdatedDate);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(200);

            // Relationships
            builder.HasOne(x => x.Dispute)
                .WithMany(d => d.Attachments)
                .HasForeignKey(x => x.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.DisputeId);
        }
    }
}
