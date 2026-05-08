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

namespace TransactionsDisputePortal.Application.Disputes.Queries.GetDisputes
{
    /// <summary>
    /// Query to get paginated list of disputes with filtering
    /// </summary>
    public class GetDisputesQuery : IRequest<PagedResult<DisputeDto>>
    {
        public Guid CustomerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }

    public class GetDisputesQueryHandler : IRequestHandler<GetDisputesQuery, PagedResult<DisputeDto>>
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly IMapper _mapper;

        public GetDisputesQueryHandler(IDisputeRepository disputeRepository, IMapper mapper)
        {
            _disputeRepository = disputeRepository ?? throw new ArgumentNullException(nameof(disputeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<DisputeDto>> Handle(GetDisputesQuery request, CancellationToken cancellationToken)
        {
            // Build filter expression
            Expression<Func<Dispute, bool>> filterExpression = d => d.CustomerId == request.CustomerId;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<DisputeStatus>(request.Status, true, out var status))
                {
                    var currentFilter = filterExpression;
                    filterExpression = d => currentFilter.Compile()(d) && d.Status == status;
                }
            }

            // Build query options for sorting and including related entities
            Func<IQueryable<Dispute>, IQueryable<Dispute>> queryOptions = query =>
            {
                // Apply sorting
                query = (request.SortBy?.ToLower(), request.SortOrder?.ToLower()) switch
                {
                    ("createddate", "asc") => query.OrderBy(d => d.CreatedDate),
                    ("status", "desc") => query.OrderByDescending(d => d.Status),
                    ("status", _) => query.OrderBy(d => d.Status),
                    _ => query.OrderByDescending(d => d.CreatedDate)
                };

                return query;
            };

            // Execute query with pagination
            var pagedDisputes = await _disputeRepository.FindAllProjectToAsync<DisputeDto>(
                filterExpression,
                request.PageNumber,
                request.PageSize,
                queryOptions,
                cancellationToken);

            return PagedResult<DisputeDto>.Create(
                pagedDisputes.TotalCount,
                pagedDisputes.PageCount,
                pagedDisputes.PageSize,
                pagedDisputes.PageNo,
                pagedDisputes.ToList());
        }
    }
}
