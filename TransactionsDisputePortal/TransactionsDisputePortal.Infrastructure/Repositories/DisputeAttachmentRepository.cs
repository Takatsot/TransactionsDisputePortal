using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Domain.Repositories;
using TransactionsDisputePortal.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Repositories
{
    public class DisputeAttachmentRepository : RepositoryBase<DisputeAttachment, DisputeAttachment, ApplicationDbContext>, IDisputeAttachmentRepository
    {
        public DisputeAttachmentRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<List<DisputeAttachment>> GetByDisputeIdAsync(Guid disputeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<DisputeAttachment>()
                .Where(a => a.DisputeId == disputeId)
                .OrderBy(a => a.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
