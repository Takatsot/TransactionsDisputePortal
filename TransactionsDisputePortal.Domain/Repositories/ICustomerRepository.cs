using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Customer entity
    /// </summary>
    public interface ICustomerRepository : IEFRepository<Customer, Customer>
    {
        Task<Customer?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
