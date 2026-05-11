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
    /// Represents a dispute filed against a transaction
    /// </summary>
    public class Dispute : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Guid CustomerId { get; private set; }
        public DisputeReason Reason { get; private set; }
        public string Description { get; private set; } = null!;
        public DisputeStatus Status { get; private set; }
        public DateTime? ResolvedDate { get; private set; }
        public string? ResolutionNotes { get; private set; }

        // Navigation properties
        public virtual Transaction Transaction { get; private set; } = null!;
        public virtual Customer Customer { get; private set; } = null!;
        public virtual ICollection<DisputeHistory> History { get; private set; } = new List<DisputeHistory>();
        public virtual ICollection<DisputeAttachment> Attachments { get; private set; } = new List<DisputeAttachment>();

        // Computed properties
        public bool IsActive => Status == DisputeStatus.Pending || Status == DisputeStatus.UnderReview;
        
        public TimeSpan? ResolutionTime => ResolvedDate.HasValue
            ? ResolvedDate.Value - CreatedDate
            : null;

        // Private constructor for EF Core
        protected Dispute()
        {
        }

        /// <summary>
        /// Factory method to create a new dispute
        /// </summary>
        public static Dispute Create(
            Guid transactionId,
            Guid customerId,
            DisputeReason reason,
            string description,
            string createdBy = "System")
        {
            if (transactionId == Guid.Empty)
                throw new ArgumentException("Transaction ID is required", nameof(transactionId));

            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID is required", nameof(customerId));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));

            if (description.Length < 20)
                throw new ArgumentException("Description must be at least 20 characters", nameof(description));

            var dispute = new Dispute
            {
                TransactionId = transactionId,
                CustomerId = customerId,
                Reason = reason,
                Description = description.Trim(),
                Status = DisputeStatus.Pending,
                CreatedBy = createdBy,
                History = new List<DisputeHistory>()
            };

            // Note: Initial history entry will be added after the dispute is saved and has an Id
            // See CreateDisputeCommandHandler for history entry creation

            return dispute;
        }

        /// <summary>
        /// Approves the dispute
        /// </summary>
        public void Approve(string notes, string approvedBy)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Only active disputes can be approved.");

            if (string.IsNullOrWhiteSpace(approvedBy))
                throw new ArgumentException("Approved by is required", nameof(approvedBy));

            Status = DisputeStatus.Approved;
            ResolutionNotes = notes?.Trim();
            ResolvedDate = DateTime.UtcNow;
            SetUpdated(approvedBy);

            AddHistoryEntry(DisputeStatus.Approved, notes ?? "Dispute approved", approvedBy);
        }

        /// <summary>
        /// Rejects the dispute
        /// </summary>
        public void Reject(string notes, string rejectedBy)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Only active disputes can be rejected.");

            if (string.IsNullOrWhiteSpace(rejectedBy))
                throw new ArgumentException("Rejected by is required", nameof(rejectedBy));

            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Rejection notes are required", nameof(notes));

            Status = DisputeStatus.Rejected;
            ResolutionNotes = notes.Trim();
            ResolvedDate = DateTime.UtcNow;
            SetUpdated(rejectedBy);

            AddHistoryEntry(DisputeStatus.Rejected, notes, rejectedBy);
        }

        /// <summary>
        /// Marks the dispute as under review
        /// </summary>
        public void MarkUnderReview(string notes, string reviewedBy)
        {
            if (Status != DisputeStatus.Pending)
                throw new BusinessRuleViolationException("Only pending disputes can be marked under review.");

            if (string.IsNullOrWhiteSpace(reviewedBy))
                throw new ArgumentException("Reviewed by is required", nameof(reviewedBy));

            Status = DisputeStatus.UnderReview;
            SetUpdated(reviewedBy);

            AddHistoryEntry(DisputeStatus.UnderReview, notes ?? "Dispute marked for review", reviewedBy);
        }

        /// <summary>
        /// Cancels the dispute
        /// </summary>
        public void Cancel(string reason, string cancelledBy)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Only active disputes can be cancelled.");

            if (string.IsNullOrWhiteSpace(cancelledBy))
                throw new ArgumentException("Cancelled by is required", nameof(cancelledBy));

            Status = DisputeStatus.Cancelled;
            ResolutionNotes = reason?.Trim() ?? "Cancelled by customer";
            ResolvedDate = DateTime.UtcNow;
            SetUpdated(cancelledBy);

            // Note: History entry will be added separately to avoid EF Core tracking issues
            // AddHistoryEntry(DisputeStatus.Cancelled, ResolutionNotes, cancelledBy);
        }

        /// <summary>
        /// Adds a history entry for this dispute
        /// </summary>
        private void AddHistoryEntry(DisputeStatus status, string notes, string changedBy)
        {
            var historyEntry = DisputeHistory.Create(Id, status, notes, changedBy);
            History.Add(historyEntry);
        }
    }
}
