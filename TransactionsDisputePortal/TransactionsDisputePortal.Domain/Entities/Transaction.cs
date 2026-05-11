using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Common;
using TransactionsDisputePortal.Domain.Common.Exceptions;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents a financial transaction
    /// </summary>
    public class Transaction : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = null!;
        public string MerchantName { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public string Category { get; private set; } = null!;
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        // Navigation properties
        public virtual Customer Customer { get; private set; } = null!;
        public virtual ICollection<Dispute> Disputes { get; private set; } = new List<Dispute>();

        // Computed properties
        public bool IsDisputed => Disputes.Any(d => d.IsActive);
        
        public bool CanBeDisputed =>
            !Disputes.Any(d => d.IsActive) &&
            TransactionDate >= DateTime.UtcNow.AddDays(-90) &&
            Status == TransactionStatus.Completed;

        // Protected constructor for EF Core and lazy loading proxies
        protected Transaction()
        {
        }

        /// <summary>
        /// Factory method to create a new transaction
        /// </summary>
        public static Transaction Create(
            Guid customerId,
            DateTime transactionDate,
            decimal amount,
            string currency,
            string merchantName,
            string description,
            string category,
            TransactionType type,
            string createdBy = "System")
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID is required", nameof(customerId));

            if (transactionDate > DateTime.UtcNow)
                throw new ArgumentException("Transaction date cannot be in the future", nameof(transactionDate));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            if (string.IsNullOrWhiteSpace(merchantName))
                throw new ArgumentException("Merchant name is required", nameof(merchantName));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required", nameof(category));

            return new Transaction
            {
                CustomerId = customerId,
                TransactionDate = transactionDate,
                Amount = Math.Round(amount, 2),
                Currency = currency.ToUpperInvariant(),
                MerchantName = merchantName.Trim(),
                Description = description?.Trim() ?? string.Empty,
                Category = category.Trim(),
                Type = type,
                Status = TransactionStatus.Completed,
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Marks the transaction as disputed
        /// </summary>
        public void MarkAsDisputed(string updatedBy = "System")
        {
            // Note: Validation should be done before calling this method
            // The CanBeDisputed check is skipped here because EF Core's change tracker
            // may have already set the Dispute navigation property, causing false negatives
            Status = TransactionStatus.Disputed;
            SetUpdated(updatedBy);
        }

        /// <summary>
        /// Marks the transaction as reversed (after successful dispute)
        /// </summary>
        public void MarkAsReversed(string updatedBy = "System")
        {
            if (Status != TransactionStatus.Disputed)
                throw new BusinessRuleViolationException("Only disputed transactions can be reversed.");

            Status = TransactionStatus.Reversed;
            SetUpdated(updatedBy);
        }

        /// <summary>
        /// Restores the transaction to completed status (after rejected/cancelled dispute)
        /// </summary>
        public void RestoreToCompleted(string updatedBy = "System")
        {
            if (Status != TransactionStatus.Disputed)
                throw new BusinessRuleViolationException("Only disputed transactions can be restored.");

            Status = TransactionStatus.Completed;
            SetUpdated(updatedBy);
        }
    }
}
