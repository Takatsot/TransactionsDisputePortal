using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Transaction entity
    /// </summary>
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            // Base entity properties
            builder.Property(t => t.Id)
                .IsRequired();

            builder.Property(t => t.CreatedDate)
                .IsRequired();

            builder.Property(t => t.UpdatedDate)
                .IsRequired(false);

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(256);

            // Transaction specific properties
            builder.Property(t => t.CustomerId)
                .IsRequired();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(t => t.MerchantName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            // Indexes
            builder.HasIndex(t => t.CustomerId)
                .HasDatabaseName("IX_Transactions_CustomerId");

            builder.HasIndex(t => t.TransactionDate)
                .HasDatabaseName("IX_Transactions_TransactionDate");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Transactions_Status");

            builder.HasIndex(t => new { t.CustomerId, t.TransactionDate })
                .HasDatabaseName("IX_Transactions_CustomerId_TransactionDate");

            // Relationships
            builder.HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Disputes)
                .WithOne(d => d.Transaction)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignore computed properties
            builder.Ignore(t => t.IsDisputed);
            builder.Ignore(t => t.CanBeDisputed);
        }
    }
}
