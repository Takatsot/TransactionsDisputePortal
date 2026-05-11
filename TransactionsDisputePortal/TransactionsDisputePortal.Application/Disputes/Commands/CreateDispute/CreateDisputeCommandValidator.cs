using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute
{
    /// <summary>
    /// Validator for CreateDisputeCommand
    /// </summary>
    public class CreateDisputeCommandValidator : AbstractValidator<CreateDisputeCommand>
    {
        public CreateDisputeCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage("Transaction ID is required.");

            RuleFor(x => x.Reason)
                .IsInEnum()
                .WithMessage("Valid dispute reason is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MinimumLength(20)
                .WithMessage("Description must be at least 20 characters.")
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}
