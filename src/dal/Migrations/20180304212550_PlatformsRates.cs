using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace dal.Migrations
{
    public partial class PlatformsRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlatformID",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PlatformCurrencyPairs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PlatformID = table.Column<int>(nullable: false),
                    SourceCurrencyID = table.Column<int>(nullable: false),
                    TargetCurrencyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformCurrencyPairs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlatformCurrencyPairs_Platforms_PlatformID",
                        column: x => x.PlatformID,
                        principalTable: "Platforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformCurrencyPairs_Currencies_SourceCurrencyID",
                        column: x => x.SourceCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformCurrencyPairs_Currencies_TargetCurrencyID",
                        column: x => x.TargetCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Close = table.Column<decimal>(nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    High = table.Column<decimal>(nullable: true),
                    Low = table.Column<decimal>(nullable: true),
                    Open = table.Column<decimal>(nullable: true),
                    PlatformID = table.Column<int>(nullable: false),
                    RateSet = table.Column<decimal>(nullable: true),
                    SourceCurrencyID = table.Column<int>(nullable: false),
                    TargetCurrencyID = table.Column<int>(nullable: false),
                    Volume = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlatformRates_Platforms_PlatformID",
                        column: x => x.PlatformID,
                        principalTable: "Platforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformRates_Currencies_SourceCurrencyID",
                        column: x => x.SourceCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformRates_Currencies_TargetCurrencyID",
                        column: x => x.TargetCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PlatformID",
                table: "Accounts",
                column: "PlatformID");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCurrencyPairs_SourceCurrencyID",
                table: "PlatformCurrencyPairs",
                column: "SourceCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCurrencyPairs_TargetCurrencyID",
                table: "PlatformCurrencyPairs",
                column: "TargetCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCurrencyPairs_PlatformID_SourceCurrencyID_TargetCurrencyID",
                table: "PlatformCurrencyPairs",
                columns: new[] { "PlatformID", "SourceCurrencyID", "TargetCurrencyID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRates_SourceCurrencyID",
                table: "PlatformRates",
                column: "SourceCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRates_TargetCurrencyID",
                table: "PlatformRates",
                column: "TargetCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRates_PlatformID_SourceCurrencyID_TargetCurrencyID_Date",
                table: "PlatformRates",
                columns: new[] { "PlatformID", "SourceCurrencyID", "TargetCurrencyID", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Name",
                table: "Platforms",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Platforms_PlatformID",
                table: "Accounts",
                column: "PlatformID",
                principalTable: "Platforms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Platforms_PlatformID",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "PlatformCurrencyPairs");

            migrationBuilder.DropTable(
                name: "PlatformRates");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_PlatformID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PlatformID",
                table: "Accounts");
        }
    }
}
