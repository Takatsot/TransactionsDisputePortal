using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransactionsDisputePortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLookupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisputeReasonLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeReasonLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisputeStatusLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatusLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypeLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypeLookups", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DisputeReasonLookups",
                columns: new[] { "Id", "Code", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "UnauthorizedTransaction", "Unauthorized Transaction", 1, true },
                    { 2, "IncorrectAmount", "Incorrect Amount", 2, true },
                    { 3, "DuplicateCharge", "Duplicate Charge", 3, true },
                    { 4, "ProductNotReceived", "Product Not Received", 4, true },
                    { 5, "ProductDefective", "Product Defective", 5, true },
                    { 6, "ServiceNotProvided", "Service Not Provided", 6, true },
                    { 7, "Fraudulent", "Fraudulent", 7, true },
                    { 99, "Other", "Other", 99, true }
                });

            migrationBuilder.InsertData(
                table: "DisputeStatusLookups",
                columns: new[] { "Id", "Code", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "Pending", "Pending", 1, true },
                    { 2, "UnderReview", "Under Review", 2, true },
                    { 3, "Approved", "Approved", 3, true },
                    { 4, "Rejected", "Rejected", 4, true },
                    { 5, "Cancelled", "Cancelled", 5, true }
                });

            migrationBuilder.InsertData(
                table: "TransactionStatusLookups",
                columns: new[] { "Id", "Code", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "Pending", "Pending", 1, true },
                    { 2, "Completed", "Completed", 2, true },
                    { 3, "Disputed", "Disputed", 3, true },
                    { 4, "Reversed", "Reversed", 4, true },
                    { 5, "Failed", "Failed", 5, true }
                });

            migrationBuilder.InsertData(
                table: "TransactionTypeLookups",
                columns: new[] { "Id", "Code", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "Debit", "Debit", 1, true },
                    { 2, "Credit", "Credit", 2, true },
                    { 3, "Refund", "Refund", 3, true },
                    { 4, "Fee", "Fee", 4, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisputeReasonLookups_Code",
                table: "DisputeReasonLookups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisputeStatusLookups_Code",
                table: "DisputeStatusLookups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionStatusLookups_Code",
                table: "TransactionStatusLookups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypeLookups_Code",
                table: "TransactionTypeLookups",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisputeReasonLookups");

            migrationBuilder.DropTable(
                name: "DisputeStatusLookups");

            migrationBuilder.DropTable(
                name: "TransactionStatusLookups");

            migrationBuilder.DropTable(
                name: "TransactionTypeLookups");
        }
    }
}
