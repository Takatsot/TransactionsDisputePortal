using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Common
{
    /// <summary>
    /// Base entity class containing common properties for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public DateTime? UpdatedDate { get; protected set; }
        public string CreatedBy { get; protected set; } = "System";
        public string? UpdatedBy { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the audit fields when entity is modified
        /// </summary>
        /// <param name="updatedBy">User who made the update</param>
        protected void SetUpdated(string updatedBy)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
