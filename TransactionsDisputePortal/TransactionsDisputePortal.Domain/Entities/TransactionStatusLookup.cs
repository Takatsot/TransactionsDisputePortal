using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Lookup table for transaction statuses
    /// </summary>
    public class TransactionStatusLookup
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
