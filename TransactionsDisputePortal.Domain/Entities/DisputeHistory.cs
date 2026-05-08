using System;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents an audit trail entry for dispute status changes
    /// </summary>
    public class DisputeHistory : BaseEntity
    {
        public Guid DisputeId { get; private set; }
        public DisputeStatus Status { get; private set; }
        public string Notes { get; private set; } = null!;
        public string ChangedBy { get; private set; } = null!;
        public DateTime ChangedDate { get; private set; }

        // Navigation properties
        public virtual Dispute Dispute { get; private set; } = null!;

        // Private constructor for EF Core
        protected DisputeHistory()
        {
        }

        /// <summary>
        /// Factory method to create a new dispute history entry
        /// </summary>
        public static DisputeHistory Create(
            Guid disputeId,
            DisputeStatus status,
            string notes,
            string changedBy)
        {
            if (disputeId == Guid.Empty)
                throw new ArgumentException("Dispute ID is required", nameof(disputeId));

            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Notes are required", nameof(notes));

            if (string.IsNullOrWhiteSpace(changedBy))
                throw new ArgumentException("Changed by is required", nameof(changedBy));

            return new DisputeHistory
            {
                DisputeId = disputeId,
                Status = status,
                Notes = notes.Trim(),
                ChangedBy = changedBy.Trim(),
                ChangedDate = DateTime.UtcNow,
                CreatedBy = changedBy.Trim()
            };
        }
    }
}
