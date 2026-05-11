using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// Response model for authentication endpoints
    /// </summary>
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
