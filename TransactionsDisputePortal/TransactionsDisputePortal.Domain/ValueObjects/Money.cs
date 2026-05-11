using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.ValueObjects
{
    /// <summary>
    /// Represents a monetary amount with currency
    /// </summary>
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money()
        {
        }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            if (currency.Length != 3)
                throw new ArgumentException("Currency must be a 3-letter ISO 4217 code", nameof(currency));

            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static bool operator >(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");

            return a.Amount > b.Amount;
        }

        public static bool operator <(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");

            return a.Amount < b.Amount;
        }

        public override string ToString() => $"{Amount:N2} {Currency}";

        public static Money Zero(string currency) => new Money(0, currency);
    }
}
