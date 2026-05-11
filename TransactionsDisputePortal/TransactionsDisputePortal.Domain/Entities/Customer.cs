using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Entities
{
    /// <summary>
    /// Represents a customer in the system
    /// </summary>
    public class Customer : BaseEntity
    {
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public virtual ICollection<Dispute> Disputes { get; private set; } = new List<Dispute>();

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";

        // Private constructor for EF Core
        protected Customer()
        {
        }

        /// <summary>
        /// Factory method to create a new customer
        /// </summary>
        public static Customer Create(string email, string passwordHash, string firstName, string lastName, string createdBy = "System")
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required", nameof(passwordHash));

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required", nameof(lastName));

            return new Customer
            {
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                IsActive = true,
                CreatedBy = createdBy,
                Transactions = new List<Transaction>(),
                Disputes = new List<Dispute>()
            };
        }

        /// <summary>
        /// Deactivates the customer account
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activates the customer account
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Updates customer information
        /// </summary>
        public void UpdateInfo(string firstName, string lastName, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            SetUpdated(updatedBy);
        }

        /// <summary>
        /// Updates customer password
        /// </summary>
        public void UpdatePassword(string passwordHash, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required", nameof(passwordHash));

            PasswordHash = passwordHash;
            SetUpdated(updatedBy);
        }
    }
}
