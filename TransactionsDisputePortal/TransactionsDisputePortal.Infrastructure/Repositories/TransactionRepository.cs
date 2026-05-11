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
    /// Repository implementation for Transaction entity
    /// </summary>
    public class TransactionRepository : RepositoryBase<Transaction, Transaction, ApplicationDbContext>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Transaction?> FindByIdWithDisputeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Transactions
                .Include(t => t.Disputes)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Transaction>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Transactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CanBeDisputedAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.Disputes)
                .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

            return transaction != null && transaction.CanBeDisputed;
        }
    }
}
