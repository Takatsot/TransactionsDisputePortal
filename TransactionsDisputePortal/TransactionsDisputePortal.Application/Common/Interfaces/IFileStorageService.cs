using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionsDisputePortal.Application.Common.Interfaces
{
    /// <summary>
    /// Service for storing and retrieving files
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves a file from a stream and returns the storage path
        /// </summary>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a file from storage
        /// </summary>
        Task DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a file stream for download
        /// </summary>
        Task<Stream> GetFileStreamAsync(string storagePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a file exists
        /// </summary>
        Task<bool> FileExistsAsync(string storagePath, CancellationToken cancellationToken = default);
    }
}
