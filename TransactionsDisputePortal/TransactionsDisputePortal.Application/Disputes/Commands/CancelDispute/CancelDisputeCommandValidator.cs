using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Disputes.Commands.CancelDispute;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Commands.CancelDispute
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
                .WithMessage("Dispute ID is required.");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}
