using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents the status of a dispute
    /// </summary>
    public enum DisputeStatus
    {
        /// <summary>
        /// Dispute has been submitted and is awaiting review
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Dispute is currently being reviewed by the team
        /// </summary>
        UnderReview = 2,

        /// <summary>
        /// Dispute was approved and customer will be refunded
        /// </summary>
        Approved = 3,

        /// <summary>
        /// Dispute was rejected after review
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Dispute was cancelled by the customer
        /// </summary>
        Cancelled = 5
    }
}
