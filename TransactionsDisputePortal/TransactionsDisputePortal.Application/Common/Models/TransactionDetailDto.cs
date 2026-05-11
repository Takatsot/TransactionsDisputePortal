using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// Detailed DTO for transaction with related dispute information
    /// </summary>
    public class TransactionDetailDto
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string MerchantName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool IsDisputed { get; set; }
        public bool CanBeDisputed { get; set; }
        public DisputeDto? Dispute { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
