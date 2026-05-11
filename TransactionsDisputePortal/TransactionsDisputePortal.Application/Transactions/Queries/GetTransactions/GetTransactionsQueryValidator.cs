using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Transactions.Queries.GetTransactions;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Transactions.Queries.GetTransactions
{
    /// <summary>
    /// Validator for GetTransactionsQuery
    /// </summary>
    public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
    {
        public GetTransactionsQueryValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum amount must be greater than or equal to 0.")
                .When(x => x.MinAmount.HasValue);

            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum amount must be greater than or equal to 0.")
                .When(x => x.MaxAmount.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinAmount.HasValue || !x.MaxAmount.HasValue || x.MinAmount.Value <= x.MaxAmount.Value)
                .WithMessage("Minimum amount must be less than or equal to maximum amount.");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be before or equal to end date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }
    }
}
