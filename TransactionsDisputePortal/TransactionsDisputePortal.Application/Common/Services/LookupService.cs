using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Services
{
    /// <summary>
    /// Service for retrieving lookup data
    /// </summary>
    public interface ILookupService
    {
        string GetDisputeReasonDescription(DisputeReason reason);
        string GetDisputeStatusDescription(DisputeStatus status);
        string GetTransactionStatusDescription(TransactionStatus status);
        string GetTransactionTypeDescription(TransactionType type);
    }
}
