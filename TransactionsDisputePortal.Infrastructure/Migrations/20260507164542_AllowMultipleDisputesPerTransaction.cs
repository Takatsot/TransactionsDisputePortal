using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionsDisputePortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleDisputesPerTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Disputes_TransactionId",
                table: "Disputes");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_TransactionId",
                table: "Disputes",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Disputes_TransactionId",
                table: "Disputes");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_TransactionId",
                table: "Disputes",
                column: "TransactionId",
                unique: true);
        }
    }
}
