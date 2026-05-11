using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// DTO for dispute data
    /// </summary>
    public class DisputeDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public TransactionSummaryDto? Transaction { get; set; }
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
