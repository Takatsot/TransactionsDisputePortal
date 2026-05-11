using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents the status of a transaction
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction is pending completion
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Transaction completed successfully
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Transaction is disputed
        /// </summary>
        Disputed = 3,

        /// <summary>
        /// Transaction was reversed/refunded
        /// </summary>
        Reversed = 4,

        /// <summary>
        /// Transaction failed
        /// </summary>
        Failed = 5
    }
}
