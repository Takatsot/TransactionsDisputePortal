using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Interfaces
{
    /// <summary>
    /// Service for hashing and verifying passwords
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }
}
