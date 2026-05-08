using Microsoft.Extensions.Diagnostics.HealthChecks;
using TransactionsDisputePortal.Application.Common.Interfaces;

namespace TransactionsDisputePortal.Api.Configuration
{
    /// <summary>
    /// Health check for file storage availability
    /// </summary>
    public class FileStorageHealthCheck : IHealthCheck
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FileStorageHealthCheck> _logger;

        public FileStorageHealthCheck(
            IFileStorageService fileStorageService,
            ILogger<FileStorageHealthCheck> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if uploads directory exists and is writable
                var testFileName = $"health-check-{Guid.NewGuid()}.tmp";
                var testContent = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("health check"));
                
                var storagePath = await _fileStorageService.SaveFileAsync(
                    testContent,
                    testFileName,
                    "health-checks",
                    cancellationToken);

                // Clean up test file
                await _fileStorageService.DeleteFileAsync(storagePath, cancellationToken);

                return HealthCheckResult.Healthy("File storage is operational");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File storage health check failed");
                return HealthCheckResult.Degraded(
                    "File storage is not fully operational",
                    ex);
            }
        }
    }
}
