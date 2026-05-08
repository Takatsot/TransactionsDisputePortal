using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Application.Common.Pagination;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Transactions.Queries.GetTransactions
{
    /// <summary>
    /// Query to get paginated list of transactions with filtering and sorting
    /// </summary>
    public class GetTransactionsQuery : IRequest<PagedResult<TransactionDto>>
    {
        public Guid CustomerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
    }

    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public GetTransactionsQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            // Build filter expression
            Expression<Func<Transaction, bool>> filterExpression = t => t.CustomerId == request.CustomerId;

            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value;
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && t.TransactionDate >= startDate;
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value;
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && t.TransactionDate <= endDate;
            }

            if (request.MinAmount.HasValue)
            {
                var minAmount = request.MinAmount.Value;
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && t.Amount >= minAmount;
            }

            if (request.MaxAmount.HasValue)
            {
                var maxAmount = request.MaxAmount.Value;
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && t.Amount <= maxAmount;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<TransactionStatus>(request.Status, true, out var status))
                {
                    var currentFilter = filterExpression;
                    filterExpression = t => currentFilter.Compile()(t) && t.Status == status;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var category = request.Category;
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && t.Category.ToLower() == category.ToLower();
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                var currentFilter = filterExpression;
                filterExpression = t => currentFilter.Compile()(t) && 
                    (t.MerchantName.ToLower().Contains(searchTerm) || 
                     t.Description.ToLower().Contains(searchTerm));
            }

            // Build query options for sorting
            Func<IQueryable<Transaction>, IQueryable<Transaction>> queryOptions = query =>
            {
                // Apply sorting
                query = (request.SortBy?.ToLower(), request.SortOrder?.ToLower()) switch
                {
                    ("amount", "desc") => query.OrderByDescending(t => t.Amount),
                    ("amount", _) => query.OrderBy(t => t.Amount),
                    ("merchantname", "desc") => query.OrderByDescending(t => t.MerchantName),
                    ("merchantname", _) => query.OrderBy(t => t.MerchantName),
                    ("transactiondate", "asc") => query.OrderBy(t => t.TransactionDate),
                    _ => query.OrderByDescending(t => t.TransactionDate)
                };

                return query;
            };

            // Execute query with pagination
            var pagedTransactions = await _transactionRepository.FindAllProjectToAsync<TransactionDto>(
                filterExpression,
                request.PageNumber,
                request.PageSize,
                queryOptions,
                cancellationToken);

            return PagedResult<TransactionDto>.Create(
                pagedTransactions.TotalCount,
                pagedTransactions.PageCount,
                pagedTransactions.PageSize,
                pagedTransactions.PageNo,
                pagedTransactions.ToList());
        }
    }
}
