using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Infrastructure.Persistence;
using TransactionsDisputePortal.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Persistence
{
    /// <summary>
    /// Seeds initial data into the database for development and testing
    /// </summary>
    public static class DatabaseSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Skip if data already exists
            if (context.Customers.Any())
            {
                return;
            }

            // Create password hasher for seed data
            IPasswordHasher passwordHasher = new PasswordHasher();
            var defaultPasswordHash = passwordHasher.HashPassword("Password123!"); // Default password for test accounts

            // Create sample customers
            var customer1 = Customer.Create(
                "testuser@email.com",
                defaultPasswordHash,
                "Thabo",
                "Kapiteni",
                "DatabaseSeeder");
            // Set a known ID for development testing
            typeof(Customer).GetProperty("Id")!.SetValue(customer1, Guid.Parse("00000000-0000-0000-0000-000000000001"));

            var customer2 = Customer.Create(
                "testuser2@email.com",
                defaultPasswordHash,
                "Jane",
                "Smith",
                "DatabaseSeeder");

            context.Customers.AddRange(customer1, customer2);
            context.SaveChanges();

            // Create sample transactions for Customer 1
            var transactions = new List<Transaction>
            {
                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-5),
                    125.50m,
                    "ZAR",
                    "Takealot",
                    "Electronics purchase - Wireless headphones",
                    "Shopping",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-10),
                    45.99m,
                    "ZAR",
                    "Checkers",
                    "Grocery shopping",
                    "Groceries",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-15),
                    89.00m,
                    "ZAR",
                    "Sasol",
                    "Fuel purchase",
                    "Transportation",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-20),
                    1500.00m,
                    "ZAR",
                    "Monthly Salary",
                    "Salary deposit",
                    "Income",
                    TransactionType.Credit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-25),
                    75.00m,
                    "ZAR",
                    "Netflix",
                    "Monthly subscription",
                    "Entertainment",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-30),
                    200.00m,
                    "ZAR",
                    "Uber",
                    "Multiple rides",
                    "Transportation",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-35),
                    50.00m,
                    "ZAR",
                    "Vida e Caffè",
                    "Coffee and snacks",
                    "Dining",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-40),
                    299.99m,
                    "ZAR",
                    "Game",
                    "Smart watch purchase",
                    "Shopping",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-45),
                    15.00m,
                    "ZAR",
                    "Bank Fee",
                    "Monthly maintenance fee",
                    "Fees",
                    TransactionType.Fee,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer1.Id,
                    DateTime.UtcNow.AddDays(-50),
                    120.00m,
                    "ZAR",
                    "TFG (The Foschini Group)",
                    "Clothing purchase",
                    "Shopping",
                    TransactionType.Debit,
                    "DatabaseSeeder")
            };

            // Create sample transactions for Customer 2
            var customer2Transactions = new List<Transaction>
            {
                Transaction.Create(
                    customer2.Id,
                    DateTime.UtcNow.AddDays(-3),
                    65.00m,
                    "ZAR",
                    "Woolworths",
                    "Organic groceries",
                    "Groceries",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer2.Id,
                    DateTime.UtcNow.AddDays(-8),
                    2000.00m,
                    "ZAR",
                    "Salary Deposit",
                    "Monthly salary",
                    "Income",
                    TransactionType.Credit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer2.Id,
                    DateTime.UtcNow.AddDays(-12),
                    89.99m,
                    "ZAR",
                    "iStore",
                    "App Store purchases",
                    "Shopping",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer2.Id,
                    DateTime.UtcNow.AddDays(-18),
                    45.00m,
                    "ZAR",
                    "Spotify",
                    "Premium subscription",
                    "Entertainment",
                    TransactionType.Debit,
                    "DatabaseSeeder"),

                Transaction.Create(
                    customer2.Id,
                    DateTime.UtcNow.AddDays(-22),
                    150.00m,
                    "ZAR",
                    "FlySafair",
                    "Flight booking",
                    "Travel",
                    TransactionType.Debit,
                    "DatabaseSeeder")
            };

            transactions.AddRange(customer2Transactions);
            context.Transactions.AddRange(transactions);
            context.SaveChanges();

            // Create a sample dispute for Customer 1
            var transactionToDispute = transactions.First(t => t.CustomerId == customer1.Id && t.MerchantName == "Game");
            transactionToDispute.MarkAsDisputed("DatabaseSeeder");

            var dispute = Dispute.Create(
                transactionToDispute.Id,
                customer1.Id,
                DisputeReason.UnauthorizedTransaction,
                "I did not authorize this purchase. My card was lost on that date and this transaction was made after I reported it missing. I have filed a police report and would like this transaction to be reversed.",
                customer1.Email);

            // Simulate dispute progression
            dispute.MarkUnderReview("Dispute assigned to fraud investigation team for review", "FraudSystem");

            context.Disputes.Add(dispute);
            context.SaveChanges();

            // Create another resolved dispute
            var anotherTransaction = transactions.First(t => t.CustomerId == customer1.Id && t.MerchantName == "Takealot");
            anotherTransaction.MarkAsDisputed("DatabaseSeeder");

            var resolvedDispute = Dispute.Create(
                anotherTransaction.Id,
                customer1.Id,
                DisputeReason.IncorrectAmount,
                "I was charged more than the price advertised on the website. The item was listed for R99.99 but I was charged R125.50. Please review and refund the difference.",
                customer1.Email);

            resolvedDispute.MarkUnderReview("Under review by customer service team", "CustomerService");
            resolvedDispute.Reject("After reviewing your case, we found that the amount charged includes shipping and handling fees which were clearly displayed during checkout. The charge is correct.", "CustomerServiceAgent");

            anotherTransaction.RestoreToCompleted("CustomerServiceAgent");

            context.Disputes.Add(resolvedDispute);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully with sample data.");
            Console.WriteLine("Test users: testuser@email.com / testuser2@email.com");
            Console.WriteLine("Default password for test accounts: Password123!");
        }
    }
}
