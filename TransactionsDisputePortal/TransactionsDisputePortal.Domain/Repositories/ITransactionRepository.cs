using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Transaction entity
    /// </summary>
    public interface ITransactionRepository : IEFRepository<Transaction, Transaction>
    {
        Task<Transaction?> FindByIdWithDisputeAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Transaction>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> CanBeDisputedAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}
