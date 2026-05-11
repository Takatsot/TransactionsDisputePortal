using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Repositories
{
    public interface IDisputeAttachmentRepository : IRepository<DisputeAttachment>
    {
        Task<List<DisputeAttachment>> GetByDisputeIdAsync(Guid disputeId, CancellationToken cancellationToken = default);
    }
}
