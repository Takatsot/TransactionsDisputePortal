using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Dispute entity
    /// </summary>
    public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
    {
        public void Configure(EntityTypeBuilder<Dispute> builder)
        {
            builder.ToTable("Disputes");

            builder.HasKey(d => d.Id);

            // Base entity properties
            builder.Property(d => d.Id)
                .IsRequired();

            builder.Property(d => d.CreatedDate)
                .IsRequired();

            builder.Property(d => d.UpdatedDate)
                .IsRequired(false);

            builder.Property(d => d.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(d => d.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(256);

            // Dispute specific properties
            builder.Property(d => d.TransactionId)
                .IsRequired();

            builder.Property(d => d.CustomerId)
                .IsRequired();

            builder.Property(d => d.Reason)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(d => d.ResolutionNotes)
                .HasMaxLength(2000);

            // Indexes
            builder.HasIndex(d => d.CustomerId)
                .HasDatabaseName("IX_Disputes_CustomerId");

            builder.HasIndex(d => d.TransactionId)
                .HasDatabaseName("IX_Disputes_TransactionId");

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_Disputes_Status");

            builder.HasIndex(d => d.CreatedDate)
                .HasDatabaseName("IX_Disputes_CreatedDate");

            builder.HasIndex(d => new { d.CustomerId, d.Status })
                .HasDatabaseName("IX_Disputes_CustomerId_Status");

            // Relationships
            builder.HasOne(d => d.Transaction)
                .WithMany(t => t.Disputes)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Customer)
                .WithMany(c => c.Disputes)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.History)
                .WithOne(h => h.Dispute)
                .HasForeignKey(h => h.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore computed properties
            builder.Ignore(d => d.IsActive);
            builder.Ignore(d => d.ResolutionTime);
        }
    }
}
