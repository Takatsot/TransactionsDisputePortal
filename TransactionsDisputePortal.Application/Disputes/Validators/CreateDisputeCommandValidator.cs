using FluentValidation;
using TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute;
using TransactionsDisputePortal.Domain.Entities;

namespace TransactionsDisputePortal.Application.Disputes.Validators
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
                .WithMessage("Customer ID is required");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage("Transaction ID is required");

            RuleFor(x => x.Reason)
                .IsInEnum()
                .WithMessage("Invalid dispute reason");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MinimumLength(20)
                .WithMessage("Description must be at least 20 characters")
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.Attachments)
                .Must(attachments => attachments == null || attachments.Count <= 5)
                .WithMessage("Cannot upload more than 5 attachments")
                .ForEach(attachment =>
                {
                    attachment.ChildRules(a =>
                    {
                        a.RuleFor(x => x.FileName)
                            .NotEmpty()
                            .WithMessage("File name is required");

                        a.RuleFor(x => x.FileSize)
                            .LessThanOrEqualTo(10 * 1024 * 1024)
                            .WithMessage("File size cannot exceed 10MB");

                        a.RuleFor(x => x.FileType)
                            .NotEmpty()
                            .WithMessage("File type is required");
                    });
                });
        }
    }
}
