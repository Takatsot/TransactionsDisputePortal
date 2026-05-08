using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TransactionsDisputePortal.Application.Common.Interfaces;

namespace TransactionsDisputePortal.Infrastructure.Services
{
    /// <summary>
    /// Local file system storage service
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public LocalFileStorageService()
        {
            // Store files in uploads directory relative to the application's working directory
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            
            // Ensure base directory exists
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is required", nameof(fileStream));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));

            // Create folder if it doesn't exist
            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream, cancellationToken);
            }

            // Return relative path
            return Path.Combine(folder, uniqueFileName);
        }

        public Task DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
                return Task.CompletedTask;

            var fullPath = Path.Combine(_basePath, storagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<Stream> GetFileStreamAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
                throw new ArgumentException("Storage path is required", nameof(storagePath));

            var fullPath = Path.Combine(_basePath, storagePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", storagePath);

            return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
        }

        public Task<bool> FileExistsAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_basePath, storagePath);
            return Task.FromResult(File.Exists(fullPath));
        }
    }
}
