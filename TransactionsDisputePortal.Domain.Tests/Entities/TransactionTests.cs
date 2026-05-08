using FluentAssertions;
using TransactionsDisputePortal.Domain.Common.Exceptions;
using TransactionsDisputePortal.Domain.Entities;
using Xunit;

namespace TransactionsDisputePortal.Domain.Tests.Entities
{
    public class TransactionTests
    {
        private readonly Guid _customerId = Guid.NewGuid();
        private const string MerchantName = "Test Merchant";
        private const string Category = "Shopping";

        [Fact]
        public void Create_WithValidData_ShouldCreateTransaction()
        {
            // Act
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-1),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            transaction.Should().NotBeNull();
            transaction.CustomerId.Should().Be(_customerId);
            transaction.Amount.Should().Be(100.50m);
            transaction.Currency.Should().Be("ZAR");
            transaction.MerchantName.Should().Be(MerchantName);
            transaction.Category.Should().Be(Category);
            transaction.Type.Should().Be(TransactionType.Debit);
            transaction.Status.Should().Be(TransactionStatus.Completed);
        }

        [Fact]
        public void Create_WithFutureDate_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(1),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Transaction date cannot be in the future*");
        }

        [Fact]
        public void Create_WithNegativeAmount_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-1),
                -50.00m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Amount must be greater than zero*");
        }

        [Fact]
        public void Create_WithEmptyCustomerId_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Transaction.Create(
                Guid.Empty,
                DateTime.UtcNow.AddDays(-1),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Customer ID is required*");
        }

        [Fact]
        public void CanBeDisputed_WithCompletedTransaction_ShouldReturnTrue()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-10),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            transaction.CanBeDisputed.Should().BeTrue();
            transaction.IsDisputed.Should().BeFalse();
        }

        [Fact]
        public void CanBeDisputed_WithOldTransaction_ShouldReturnFalse()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-91), // Older than 90 days
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Assert
            transaction.CanBeDisputed.Should().BeFalse();
        }

        [Fact]
        public void MarkAsDisputed_ShouldUpdateStatus()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-10),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Act
            transaction.MarkAsDisputed("customer@email.com");

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Disputed);
        }

        [Fact]
        public void MarkAsReversed_FromDisputedStatus_ShouldUpdateStatus()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-10),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");
            transaction.MarkAsDisputed("customer@email.com");

            // Act
            transaction.MarkAsReversed("agent@email.com");

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Reversed);
        }

        [Fact]
        public void MarkAsReversed_FromCompletedStatus_ShouldThrowBusinessRuleViolationException()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-10),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");

            // Act
            var act = () => transaction.MarkAsReversed("agent@email.com");

            // Assert
            act.Should().Throw<BusinessRuleViolationException>()
                .WithMessage("*Only disputed transactions can be reversed*");
        }

        [Fact]
        public void RestoreToCompleted_FromDisputedStatus_ShouldUpdateStatus()
        {
            // Arrange
            var transaction = Transaction.Create(
                _customerId,
                DateTime.UtcNow.AddDays(-10),
                100.50m,
                "ZAR",
                MerchantName,
                "Test purchase",
                Category,
                TransactionType.Debit,
                "system");
            transaction.MarkAsDisputed("customer@email.com");

            // Act
            transaction.RestoreToCompleted("agent@email.com");

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Completed);
        }
    }
}
