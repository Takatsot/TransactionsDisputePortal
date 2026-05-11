using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// Basic transaction information for embedding in other DTOs
    /// </summary>
    public class TransactionSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string MerchantName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
