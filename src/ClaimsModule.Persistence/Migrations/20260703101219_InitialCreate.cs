using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClaimsModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauseOfLossCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauseOfLossCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MockUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PolicyholderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CoverageLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauseOfLossCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LossDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReportedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_CauseOfLossCodes_CauseOfLossCodeId",
                        column: x => x.CauseOfLossCodeId,
                        principalTable: "CauseOfLossCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_MockUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "MockUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogEntries_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogEntries_MockUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "MockUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimParties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimParties_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reserves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GlPostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GlPostingReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reserves_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reserves_MockUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "MockUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reserves_MockUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "MockUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reserves_MockUsers_RejectedByUserId",
                        column: x => x.RejectedByUserId,
                        principalTable: "MockUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskObjects_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CauseOfLossCodes",
                columns: new[] { "Id", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000001"), "COLLISION", "Collision or impact damage", true },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000002"), "THEFT", "Theft or attempted theft", true },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000003"), "FIRE", "Fire or smoke damage", true },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000004"), "WATER_DAMAGE", "Water escape or flood damage", true },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000005"), "NATURAL_EVENT", "Storm, hail, earthquake, or other natural event", true }
                });

            migrationBuilder.InsertData(
                table: "MockUsers",
                columns: new[] { "Id", "DisplayName", "IsActive", "Role" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), "Demo Handler", true, "Handler" },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), "Demo Supervisor", true, "Supervisor" },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), "Demo Manager", true, "Manager" }
                });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "Id", "CoverageLimit", "Currency", "EffectiveFrom", "EffectiveTo", "PolicyNumber", "PolicyholderName", "ProductType", "Status" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 50000m, "USD", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), "POL-AUTO-1001", "Olena Petrenko", "Auto", "Active" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 150000m, "USD", new DateOnly(2026, 2, 1), new DateOnly(2027, 1, 31), "POL-HOME-2001", "Dmytro Kovalenko", "Home", "Active" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 25000m, "USD", new DateOnly(2026, 6, 1), new DateOnly(2026, 8, 31), "POL-TRAVEL-3001", "Sasha Agapov", "Travel", "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ActorUserId",
                table: "AuditLogEntries",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ClaimId_CreatedAtUtc",
                table: "AuditLogEntries",
                columns: new[] { "ClaimId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CauseOfLossCodes_Code",
                table: "CauseOfLossCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimParties_ClaimId",
                table: "ClaimParties",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CauseOfLossCodeId",
                table: "Claims",
                column: "CauseOfLossCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CreatedByUserId",
                table: "Claims",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicyNumber",
                table: "Policies",
                column: "PolicyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_ApprovedByUserId",
                table: "Reserves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_ClaimId",
                table: "Reserves",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_CreatedByUserId",
                table: "Reserves",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_RejectedByUserId",
                table: "Reserves",
                column: "RejectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskObjects_ClaimId",
                table: "RiskObjects",
                column: "ClaimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEntries");

            migrationBuilder.DropTable(
                name: "ClaimParties");

            migrationBuilder.DropTable(
                name: "Reserves");

            migrationBuilder.DropTable(
                name: "RiskObjects");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "CauseOfLossCodes");

            migrationBuilder.DropTable(
                name: "MockUsers");

            migrationBuilder.DropTable(
                name: "Policies");
        }
    }
}
