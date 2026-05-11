using FluentAssertions;
using TransactionsDisputePortal.Domain.Common.Exceptions;
using TransactionsDisputePortal.Domain.Entities;
using Xunit;

namespace TransactionsDisputePortal.Domain.Tests.Entities
{
    public class DisputeTests
    {
        private readonly Guid _transactionId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private const string ValidDescription = "This is a valid dispute description with at least 20 characters";

        [Fact]
        public void Create_WithValidData_ShouldCreateDispute()
        {
            // Act
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Assert
            dispute.Should().NotBeNull();
            dispute.TransactionId.Should().Be(_transactionId);
            dispute.CustomerId.Should().Be(_customerId);
            dispute.Reason.Should().Be(DisputeReason.UnauthorizedTransaction);
            dispute.Description.Should().Be(ValidDescription);
            dispute.Status.Should().Be(DisputeStatus.Pending);
            dispute.IsActive.Should().BeTrue();
            dispute.ResolvedDate.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyTransactionId_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Dispute.Create(
                Guid.Empty,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Transaction ID is required*");
        }

        [Fact]
        public void Create_WithEmptyCustomerId_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Dispute.Create(
                _transactionId,
                Guid.Empty,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Customer ID is required*");
        }

        [Fact]
        public void Create_WithShortDescription_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                "Too short",
                "test@example.com");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Description must be at least 20 characters*");
        }

        [Fact]
        public void Create_WithNullDescription_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                null!,
                "test@example.com");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Description is required*");
        }

        [Fact]
        public void MarkUnderReview_WithValidNotes_ShouldUpdateStatus()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Act
            dispute.MarkUnderReview("Under review by fraud team", "fraud_agent");

            // Assert
            dispute.Status.Should().Be(DisputeStatus.UnderReview);
            dispute.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Cancel_WithActiveDispute_ShouldCancelSuccessfully()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Act
            dispute.Cancel("Customer changed their mind", "customer@email.com");

            // Assert
            dispute.Status.Should().Be(DisputeStatus.Cancelled);
            dispute.IsActive.Should().BeFalse();
            dispute.ResolvedDate.Should().NotBeNull();
            dispute.ResolvedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            dispute.ResolutionNotes.Should().Be("Customer changed their mind");
        }

        [Fact]
        public void Cancel_WithInactiveDispute_ShouldThrowBusinessRuleViolationException()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");
            
            dispute.Approve("Approved", "agent@email.com");

            // Act
            var act = () => dispute.Cancel("Trying to cancel", "customer@email.com");

            // Assert
            act.Should().Throw<BusinessRuleViolationException>()
                .WithMessage("*Only active disputes can be cancelled*");
        }

        [Fact]
        public void Approve_WithActiveDispute_ShouldApproveSuccessfully()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Act
            dispute.Approve("Investigation confirmed unauthorized transaction", "fraud_agent");

            // Assert
            dispute.Status.Should().Be(DisputeStatus.Approved);
            dispute.IsActive.Should().BeFalse();
            dispute.ResolvedDate.Should().NotBeNull();
            dispute.ResolutionNotes.Should().Contain("Investigation confirmed");
        }

        [Fact]
        public void Reject_WithActiveDispute_ShouldRejectSuccessfully()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Act
            dispute.Reject("Evidence shows transaction was authorized", "fraud_agent");

            // Assert
            dispute.Status.Should().Be(DisputeStatus.Rejected);
            dispute.IsActive.Should().BeFalse();
            dispute.ResolvedDate.Should().NotBeNull();
        }

        [Fact]
        public void ResolutionTime_WhenResolved_ShouldReturnTimeSpan()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Act
            dispute.Approve("Resolved", "agent@email.com");

            // Assert
            dispute.ResolutionTime.Should().NotBeNull();
            dispute.ResolutionTime.Value.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
        }

        [Fact]
        public void ResolutionTime_WhenNotResolved_ShouldReturnNull()
        {
            // Arrange
            var dispute = Dispute.Create(
                _transactionId,
                _customerId,
                DisputeReason.UnauthorizedTransaction,
                ValidDescription,
                "test@example.com");

            // Assert
            dispute.ResolutionTime.Should().BeNull();
        }
    }
}
