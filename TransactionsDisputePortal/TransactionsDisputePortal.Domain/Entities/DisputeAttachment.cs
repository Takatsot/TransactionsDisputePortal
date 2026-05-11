using System;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Common;
using TransactionsDisputePortal.Domain.Common.Exceptions;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents an attachment associated with a dispute (e.g., receipts, statements)
    /// </summary>
    public class DisputeAttachment : BaseEntity
    {
        private DisputeAttachment()
        {
            // Required for EF Core
        }

        public Guid DisputeId { get; private set; }
        public string FileName { get; private set; } = null!;
        public string FileType { get; private set; } = null!;
        public long FileSize { get; private set; }
        public string StoragePath { get; private set; } = null!;

        // Navigation property
        public virtual Dispute Dispute { get; private set; } = null!;

        /// <summary>
        /// Creates a new dispute attachment
        /// </summary>
        public static DisputeAttachment Create(
            Guid disputeId,
            string fileName,
            string fileType,
            long fileSize,
            string storagePath,
            string createdBy)
        {
            if (disputeId == Guid.Empty)
                throw new ArgumentException("Dispute ID is required", nameof(disputeId));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));

            if (string.IsNullOrWhiteSpace(fileType))
                throw new ArgumentException("File type is required", nameof(fileType));

            if (fileSize <= 0)
                throw new ArgumentException("File size must be greater than zero", nameof(fileSize));

            if (string.IsNullOrWhiteSpace(storagePath))
                throw new ArgumentException("Storage path is required", nameof(storagePath));

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("Created by is required", nameof(createdBy));

            var attachment = new DisputeAttachment
            {
                Id = Guid.NewGuid(),
                DisputeId = disputeId,
                FileName = fileName.Trim(),
                FileType = fileType.Trim(),
                FileSize = fileSize,
                StoragePath = storagePath.Trim(),
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            return attachment;
        }
    }
}
