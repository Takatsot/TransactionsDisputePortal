using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using TransactionsDisputePortal.Domain.Common.Interfaces;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Infrastructure.Persistence.Configurations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace TransactionsDisputePortal.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Dispute> Disputes { get; set; } = null!;
        public DbSet<DisputeHistory> DisputeHistory { get; set; } = null!;
        
        // Lookup tables
        public DbSet<DisputeReasonLookup> DisputeReasonLookups { get; set; } = null!;
        public DbSet<DisputeStatusLookup> DisputeStatusLookups { get; set; } = null!;
        public DbSet<TransactionStatusLookup> TransactionStatusLookups { get; set; } = null!;
        public DbSet<TransactionTypeLookup> TransactionTypeLookups { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new DisputeConfiguration());
            modelBuilder.ApplyConfiguration(new DisputeHistoryConfiguration());
            
            // Apply lookup table configurations
            modelBuilder.ApplyConfiguration(new DisputeReasonLookupConfiguration());
            modelBuilder.ApplyConfiguration(new DisputeStatusLookupConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionStatusLookupConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionTypeLookupConfiguration());

            ConfigureModel(modelBuilder);
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed DisputeReason lookup data
            modelBuilder.Entity<DisputeReasonLookup>().HasData(
                new DisputeReasonLookup { Id = 1, Code = "UnauthorizedTransaction", Description = "Unauthorized Transaction", DisplayOrder = 1, IsActive = true },
                new DisputeReasonLookup { Id = 2, Code = "IncorrectAmount", Description = "Incorrect Amount", DisplayOrder = 2, IsActive = true },
                new DisputeReasonLookup { Id = 3, Code = "DuplicateCharge", Description = "Duplicate Charge", DisplayOrder = 3, IsActive = true },
                new DisputeReasonLookup { Id = 4, Code = "ProductNotReceived", Description = "Product Not Received", DisplayOrder = 4, IsActive = true },
                new DisputeReasonLookup { Id = 5, Code = "ProductDefective", Description = "Product Defective", DisplayOrder = 5, IsActive = true },
                new DisputeReasonLookup { Id = 6, Code = "ServiceNotProvided", Description = "Service Not Provided", DisplayOrder = 6, IsActive = true },
                new DisputeReasonLookup { Id = 7, Code = "Fraudulent", Description = "Fraudulent", DisplayOrder = 7, IsActive = true },
                new DisputeReasonLookup { Id = 99, Code = "Other", Description = "Other", DisplayOrder = 99, IsActive = true }
            );

            // Seed DisputeStatus lookup data
            modelBuilder.Entity<DisputeStatusLookup>().HasData(
                new DisputeStatusLookup { Id = 1, Code = "Pending", Description = "Pending", DisplayOrder = 1, IsActive = true },
                new DisputeStatusLookup { Id = 2, Code = "UnderReview", Description = "Under Review", DisplayOrder = 2, IsActive = true },
                new DisputeStatusLookup { Id = 3, Code = "Approved", Description = "Approved", DisplayOrder = 3, IsActive = true },
                new DisputeStatusLookup { Id = 4, Code = "Rejected", Description = "Rejected", DisplayOrder = 4, IsActive = true },
                new DisputeStatusLookup { Id = 5, Code = "Cancelled", Description = "Cancelled", DisplayOrder = 5, IsActive = true }
            );

            // Seed TransactionStatus lookup data
            modelBuilder.Entity<TransactionStatusLookup>().HasData(
                new TransactionStatusLookup { Id = 1, Code = "Pending", Description = "Pending", DisplayOrder = 1, IsActive = true },
                new TransactionStatusLookup { Id = 2, Code = "Completed", Description = "Completed", DisplayOrder = 2, IsActive = true },
                new TransactionStatusLookup { Id = 3, Code = "Disputed", Description = "Disputed", DisplayOrder = 3, IsActive = true },
                new TransactionStatusLookup { Id = 4, Code = "Reversed", Description = "Reversed", DisplayOrder = 4, IsActive = true },
                new TransactionStatusLookup { Id = 5, Code = "Failed", Description = "Failed", DisplayOrder = 5, IsActive = true }
            );

            // Seed TransactionType lookup data
            modelBuilder.Entity<TransactionTypeLookup>().HasData(
                new TransactionTypeLookup { Id = 1, Code = "Debit", Description = "Debit", DisplayOrder = 1, IsActive = true },
                new TransactionTypeLookup { Id = 2, Code = "Credit", Description = "Credit", DisplayOrder = 2, IsActive = true },
                new TransactionTypeLookup { Id = 3, Code = "Refund", Description = "Refund", DisplayOrder = 3, IsActive = true },
                new TransactionTypeLookup { Id = 4, Code = "Fee", Description = "Fee", DisplayOrder = 4, IsActive = true }
            );
        }
    }
}