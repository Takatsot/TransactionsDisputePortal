using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Interfaces
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a JWT token for a user
        /// </summary>
        string GenerateToken(Guid userId, string email, string firstName, string lastName);
    }
}
