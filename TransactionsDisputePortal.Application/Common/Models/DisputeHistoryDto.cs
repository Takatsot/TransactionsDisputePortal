using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// DTO for dispute history entry
    /// </summary>
    public class DisputeHistoryDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = null!;
        public string StatusDescription { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public string ChangedBy { get; set; } = null!;
        public DateTime ChangedDate { get; set; }
    }
}
