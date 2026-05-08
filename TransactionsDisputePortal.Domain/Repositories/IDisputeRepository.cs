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
    /// Repository interface for Dispute entity
    /// </summary>
    public interface IDisputeRepository : IEFRepository<Dispute, Dispute>
    {
        Task<Dispute?> FindByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Dispute?> FindByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task<bool> HasActiveDisputeAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task<List<Dispute>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}
