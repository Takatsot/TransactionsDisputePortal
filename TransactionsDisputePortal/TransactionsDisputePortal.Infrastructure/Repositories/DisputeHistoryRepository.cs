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
    /// <summary>
    /// Repository implementation for DisputeHistory entity
    /// </summary>
    public class DisputeHistoryRepository : RepositoryBase<DisputeHistory, DisputeHistory, ApplicationDbContext>, IDisputeHistoryRepository
    {
        public DisputeHistoryRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<List<DisputeHistory>> FindByDisputeIdAsync(Guid disputeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.DisputeHistory
                .Where(h => h.DisputeId == disputeId)
                .OrderBy(h => h.ChangedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
