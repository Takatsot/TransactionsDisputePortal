using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using TransactionsDisputePortal.Application.Common.Services;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for retrieving lookup data from the database
    /// </summary>
    public class LookupService : ILookupService
    {
        private readonly Dictionary<int, string> _disputeReasons;
        private readonly Dictionary<int, string> _disputeStatuses;
        private readonly Dictionary<int, string> _transactionStatuses;
        private readonly Dictionary<int, string> _transactionTypes;

        public LookupService(IServiceProvider serviceProvider)
        {
            // Load lookup data into memory using a temporary scope
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                _disputeReasons = context.DisputeReasonLookups
                    .Where(l => l.IsActive)
                    .ToDictionary(l => l.Id, l => l.Description);

                _disputeStatuses = context.DisputeStatusLookups
                    .Where(l => l.IsActive)
                    .ToDictionary(l => l.Id, l => l.Description);

                _transactionStatuses = context.TransactionStatusLookups
                    .Where(l => l.IsActive)
                    .ToDictionary(l => l.Id, l => l.Description);

                _transactionTypes = context.TransactionTypeLookups
                    .Where(l => l.IsActive)
                    .ToDictionary(l => l.Id, l => l.Description);
            }
        }

        public string GetDisputeReasonDescription(DisputeReason reason)
        {
            return _disputeReasons.TryGetValue((int)reason, out var description) 
                ? description 
                : reason.ToString();
        }

        public string GetDisputeStatusDescription(DisputeStatus status)
        {
            return _disputeStatuses.TryGetValue((int)status, out var description) 
                ? description 
                : status.ToString();
        }

        public string GetTransactionStatusDescription(TransactionStatus status)
        {
            return _transactionStatuses.TryGetValue((int)status, out var description) 
                ? description 
                : status.ToString();
        }

        public string GetTransactionTypeDescription(TransactionType type)
        {
            return _transactionTypes.TryGetValue((int)type, out var description) 
                ? description 
                : type.ToString();
        }
    }
}

