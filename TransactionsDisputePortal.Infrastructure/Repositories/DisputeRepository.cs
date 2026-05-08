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
    /// Repository implementation for Dispute entity
    /// </summary>
    public class DisputeRepository : RepositoryBase<Dispute, Dispute, ApplicationDbContext>, IDisputeRepository
    {
        public DisputeRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Dispute?> FindByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Disputes
                .Include(d => d.History)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<Dispute?> FindByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Disputes
                .Include(d => d.Transaction)
                .FirstOrDefaultAsync(d => d.TransactionId == transactionId, cancellationToken);
        }

        public async Task<bool> HasActiveDisputeAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Disputes
                .AnyAsync(d => d.TransactionId == transactionId && 
                              (d.Status == DisputeStatus.Pending || d.Status == DisputeStatus.UnderReview),
                         cancellationToken);
        }

        public async Task<List<Dispute>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Disputes
                .Include(d => d.Transaction)
                .Where(d => d.CustomerId == customerId)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
