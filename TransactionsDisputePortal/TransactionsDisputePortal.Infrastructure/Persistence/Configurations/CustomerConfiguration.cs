using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Customer entity
    /// </summary>
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            // Base entity properties
            builder.Property(c => c.Id)
                .IsRequired();

            builder.Property(c => c.CreatedDate)
                .IsRequired();

            builder.Property(c => c.UpdatedDate)
                .IsRequired(false);

            builder.Property(c => c.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(256);

            // Customer specific properties
            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.IsActive)
                .IsRequired();

            // Indexes
            builder.HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("IX_Customers_Email");

            // Relationships
            builder.HasMany(c => c.Transactions)
                .WithOne(t => t.Customer)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Disputes)
                .WithOne(d => d.Customer)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignore computed properties
            builder.Ignore(c => c.FullName);
        }
    }
}
