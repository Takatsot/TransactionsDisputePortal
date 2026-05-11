using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for DisputeStatusLookup entity
    /// </summary>
    public class DisputeStatusLookupConfiguration : IEntityTypeConfiguration<DisputeStatusLookup>
    {
        public void Configure(EntityTypeBuilder<DisputeStatusLookup> builder)
        {
            builder.ToTable("DisputeStatusLookups");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever(); // Use enum values as IDs

            builder.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(l => l.DisplayOrder)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(l => l.Code)
                .IsUnique();
        }
    }
}
