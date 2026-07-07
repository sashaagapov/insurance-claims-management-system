using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimSequenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClaimNumber",
                table: "Claims");

            migrationBuilder.AddColumn<int>(
                name: "ClaimSequenceNumber",
                table: "Claims",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "ClaimNumber",
                table: "Claims",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                computedColumnSql: "'CLM-' + CONVERT(char(4), DATEPART(year, [CreatedAtUtc])) + '-' + RIGHT('0000000' + CONVERT(varchar(7), [ClaimSequenceNumber]), 7)",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims",
                column: "ClaimNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClaimNumber",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClaimSequenceNumber",
                table: "Claims");

            migrationBuilder.AddColumn<string>(
                name: "ClaimNumber",
                table: "Claims",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims",
                column: "ClaimNumber",
                unique: true);
        }
    }
}
