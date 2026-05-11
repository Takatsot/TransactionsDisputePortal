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
    /// Repository interface for DisputeHistory entity
    /// </summary>
    public interface IDisputeHistoryRepository : IEFRepository<DisputeHistory, DisputeHistory>
    {
        Task<List<DisputeHistory>> FindByDisputeIdAsync(Guid disputeId, CancellationToken cancellationToken = default);
    }
}
