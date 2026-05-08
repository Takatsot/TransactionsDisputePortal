using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents the reason for disputing a transaction
    /// </summary>
    public enum DisputeReason
    {
        /// <summary>
        /// Transaction was not authorized by the customer
        /// </summary>
        UnauthorizedTransaction = 1,

        /// <summary>
        /// The charged amount is incorrect
        /// </summary>
        IncorrectAmount = 2,

        /// <summary>
        /// The transaction was charged multiple times
        /// </summary>
        DuplicateCharge = 3,

        /// <summary>
        /// Product or service was not received
        /// </summary>
        ProductNotReceived = 4,

        /// <summary>
        /// Product received was defective or damaged
        /// </summary>
        ProductDefective = 5,

        /// <summary>
        /// Service was not provided as agreed
        /// </summary>
        ServiceNotProvided = 6,

        /// <summary>
        /// Suspected fraudulent transaction
        /// </summary>
        Fraudulent = 7,

        /// <summary>
        /// Other reason not listed above
        /// </summary>
        Other = 99
    }
}
