using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionsDisputePortal.Infrastructure.Persistence;

namespace TransactionsDisputePortal.Api.Controllers
{
    /// <summary>
    /// API endpoints for retrieving lookup/reference data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LookupsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public LookupsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all dispute reasons for dropdown/selection
        /// </summary>
        [HttpGet("dispute-reasons")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisputeReasons()
        {
            var reasons = await _dbContext.DisputeReasonLookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .Select(l => new LookupItemDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    DisplayOrder = l.DisplayOrder
                })
                .ToListAsync();

            return Ok(reasons);
        }

        /// <summary>
        /// Get all dispute statuses for dropdown/selection
        /// </summary>
        [HttpGet("dispute-statuses")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisputeStatuses()
        {
            var statuses = await _dbContext.DisputeStatusLookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .Select(l => new LookupItemDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    DisplayOrder = l.DisplayOrder
                })
                .ToListAsync();

            return Ok(statuses);
        }

        /// <summary>
        /// Get all transaction statuses for dropdown/selection
        /// </summary>
        [HttpGet("transaction-statuses")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionStatuses()
        {
            var statuses = await _dbContext.TransactionStatusLookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .Select(l => new LookupItemDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    DisplayOrder = l.DisplayOrder
                })
                .ToListAsync();

            return Ok(statuses);
        }

        /// <summary>
        /// Get all transaction types for dropdown/selection
        /// </summary>
        [HttpGet("transaction-types")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionTypes()
        {
            var types = await _dbContext.TransactionTypeLookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .Select(l => new LookupItemDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    DisplayOrder = l.DisplayOrder
                })
                .ToListAsync();

            return Ok(types);
        }

        /// <summary>
        /// Get all lookups in one call (for initialization)
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(AllLookupsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLookups()
        {
            var result = new AllLookupsDto
            {
                DisputeReasons = await _dbContext.DisputeReasonLookups
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.DisplayOrder)
                    .Select(l => new LookupItemDto
                    {
                        Id = l.Id,
                        Code = l.Code,
                        Description = l.Description,
                        DisplayOrder = l.DisplayOrder
                    })
                    .ToListAsync(),

                DisputeStatuses = await _dbContext.DisputeStatusLookups
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.DisplayOrder)
                    .Select(l => new LookupItemDto
                    {
                        Id = l.Id,
                        Code = l.Code,
                        Description = l.Description,
                        DisplayOrder = l.DisplayOrder
                    })
                    .ToListAsync(),

                TransactionStatuses = await _dbContext.TransactionStatusLookups
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.DisplayOrder)
                    .Select(l => new LookupItemDto
                    {
                        Id = l.Id,
                        Code = l.Code,
                        Description = l.Description,
                        DisplayOrder = l.DisplayOrder
                    })
                    .ToListAsync(),

                TransactionTypes = await _dbContext.TransactionTypeLookups
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.DisplayOrder)
                    .Select(l => new LookupItemDto
                    {
                        Id = l.Id,
                        Code = l.Code,
                        Description = l.Description,
                        DisplayOrder = l.DisplayOrder
                    })
                    .ToListAsync()
            };

            return Ok(result);
        }
    }

    public class LookupItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class AllLookupsDto
    {
        public List<LookupItemDto> DisputeReasons { get; set; } = new();
        public List<LookupItemDto> DisputeStatuses { get; set; } = new();
        public List<LookupItemDto> TransactionStatuses { get; set; } = new();
        public List<LookupItemDto> TransactionTypes { get; set; } = new();
    }
}
