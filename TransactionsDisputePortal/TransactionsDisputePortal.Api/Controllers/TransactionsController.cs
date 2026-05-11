using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Application.Common.Pagination;
using TransactionsDisputePortal.Application.Transactions.Queries.GetTransactionById;
using TransactionsDisputePortal.Application.Transactions.Queries.GetTransactions;
using TransactionsDisputePortal.Api.Services;
using TransactionsDisputePortal.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Api.Controllers
{
    /// <summary>
    /// Controller for managing customer transactions
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public TransactionsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        /// <summary>
        /// Get paginated list of transactions with optional filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 100)</param>
        /// <param name="sortBy">Sort field (transactiondate, amount, merchantname)</param>
        /// <param name="sortOrder">Sort order (asc, desc)</param>
        /// <param name="searchTerm">Search in merchant name or description</param>
        /// <param name="startDate">Filter by start date</param>
        /// <param name="endDate">Filter by end date</param>
        /// <param name="minAmount">Filter by minimum amount</param>
        /// <param name="maxAmount">Filter by maximum amount</param>
        /// <param name="status">Filter by transaction status</param>
        /// <param name="category">Filter by category</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of transactions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null,
            [FromQuery] string? status = null,
            [FromQuery] string? category = null,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var id) ? id : Guid.Empty;

            var query = new GetTransactionsQuery
            {
                CustomerId = customerId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                SearchTerm = searchTerm,
                StartDate = startDate,
                EndDate = endDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Status = status,
                Category = category
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TransactionDetailDto>> GetTransactionById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var cId) ? cId : Guid.Empty;

            var query = new GetTransactionByIdQuery
            {
                Id = id,
                CustomerId = customerId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
