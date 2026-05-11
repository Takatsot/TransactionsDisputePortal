using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// Detailed DTO for dispute with full transaction information
    /// </summary>
    public class DisputeDetailDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public TransactionSummaryDto Transaction { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string ReasonDescription { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string StatusDescription { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public bool IsActive { get; set; }
    }
}
