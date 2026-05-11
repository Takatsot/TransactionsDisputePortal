using FluentValidation;
using TransactionsDisputePortal.Application.Disputes.Commands.CancelDispute;

namespace TransactionsDisputePortal.Application.Disputes.Validators
{
    /// <summary>
    /// Validator for CancelDisputeCommand
    /// </summary>
    public class CancelDisputeCommandValidator : AbstractValidator<CancelDisputeCommand>
    {
        public CancelDisputeCommandValidator()
        {
            RuleFor(x => x.DisputeId)
                .NotEmpty()
                .WithMessage("Dispute ID is required");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}
