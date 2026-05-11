using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Application.Common.Pagination;
using TransactionsDisputePortal.Application.Disputes.Commands.CancelDispute;
using TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute;
using TransactionsDisputePortal.Application.Disputes.Queries.GetDisputeById;
using TransactionsDisputePortal.Application.Disputes.Queries.GetDisputeHistory;
using TransactionsDisputePortal.Application.Disputes.Queries.GetDisputes;
using TransactionsDisputePortal.Api.Services;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Api.Controllers
{
    /// <summary>
    /// Controller for managing transaction disputes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DisputesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public DisputesController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        /// <summary>
        /// Get paginated list of disputes with optional filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 100)</param>
        /// <param name="status">Filter by dispute status (pending, underreview, approved, rejected, cancelled)</param>
        /// <param name="sortBy">Sort field (createddate, status)</param>
        /// <param name="sortOrder">Sort order (asc, desc)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of disputes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DisputeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<DisputeDto>>> GetDisputes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var id) ? id : Guid.Empty;

            var query = new GetDisputesQuery
            {
                CustomerId = customerId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific dispute by ID
        /// </summary>
        /// <param name="id">Dispute ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dispute details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DisputeDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DisputeDetailDto>> GetDisputeById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var cId) ? cId : Guid.Empty;

            var query = new GetDisputeByIdQuery
            {
                Id = id,
                CustomerId = customerId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new dispute for a transaction
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <param name="reason">Dispute reason</param>
        /// <param name="description">Dispute description</param>
        /// <param name="attachments">Optional file attachments (receipts, statements, etc.)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created dispute</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DisputeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DisputeDto>> CreateDispute(
            [FromForm] Guid transactionId,
            [FromForm] DisputeReason reason,
            [FromForm] string description,
            [FromForm] List<IFormFile>? attachments,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var cId) ? cId : Guid.Empty;

            var command = new CreateDisputeCommand
            {
                CustomerId = customerId,
                TransactionId = transactionId,
                Reason = reason,
                Description = description,
                Attachments = new List<TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute.AttachmentInfo>()
            };

            // Save files and add attachment info
            if (attachments != null && attachments.Count > 0)
            {
                var fileStorageService = HttpContext.RequestServices.GetRequiredService<TransactionsDisputePortal.Application.Common.Interfaces.IFileStorageService>();
                
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        // Validate file size (max 10MB)
                        if (file.Length > 10 * 1024 * 1024)
                        {
                            return BadRequest($"File '{file.FileName}' exceeds maximum size of 10MB");
                        }

                        // Save file
                        var storagePath = await fileStorageService.SaveFileAsync(file.OpenReadStream(), file.FileName, "disputes", cancellationToken);
                        
                        command.Attachments.Add(new TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute.AttachmentInfo
                        {
                            FileName = file.FileName,
                            FileType = file.ContentType ?? "application/octet-stream",
                            FileSize = file.Length,
                            StoragePath = storagePath
                        });
                    }
                }
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetDisputeById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Cancel an existing dispute
        /// </summary>
        /// <param name="id">Dispute ID</param>
        /// <param name="request">Cancellation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated dispute</returns>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(typeof(DisputeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DisputeDto>> CancelDispute(
            Guid id,
            [FromBody] CancelDisputeRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var cId) ? cId : Guid.Empty;

            var command = new CancelDisputeCommand
            {
                DisputeId = id,
                CustomerId = customerId,
                Reason = request.Reason
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the history of a dispute
        /// </summary>
        /// <param name="id">Dispute ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of dispute history entries</returns>
        [HttpGet("{id}/history")]
        [ProducesResponseType(typeof(List<DisputeHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<DisputeHistoryDto>>> GetDisputeHistory(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _currentUserService.GetAsync();
            var customerId = Guid.TryParse(user?.Id, out var cId) ? cId : Guid.Empty;

            var query = new GetDisputeHistoryQuery
            {
                DisputeId = id,
                CustomerId = customerId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }

    /// <summary>
    /// Request model for creating a dispute
    /// </summary>
    public class CreateDisputeRequest
    {
        public Guid TransactionId { get; set; }
        public DisputeReason Reason { get; set; }
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Request model for cancelling a dispute
    /// </summary>
    public class CancelDisputeRequest
    {
        public string? Reason { get; set; }
    }
}
