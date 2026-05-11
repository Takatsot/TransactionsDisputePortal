using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Models
{
    /// <summary>
    /// Request model for user registration
    /// </summary>
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        public string LastName { get; set; } = null!;
    }
}
