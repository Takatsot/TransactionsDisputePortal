using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents the type of financial transaction
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Money debited from account (outgoing)
        /// </summary>
        Debit = 1,

        /// <summary>
        /// Money credited to account (incoming)
        /// </summary>
        Credit = 2,

        /// <summary>
        /// Refund transaction
        /// </summary>
        Refund = 3,

        /// <summary>
        /// Bank or service fee
        /// </summary>
        Fee = 4
    }
}
