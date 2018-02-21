using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace dal.Migrations
{
    public partial class CurrencyRound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CssImagePath",
                table: "Currencies",
                newName: "CurrencySymbol");

            migrationBuilder.AddColumn<int>(
                name: "RoundToDecimals",
                table: "Currencies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundToDecimals",
                table: "Currencies");

            migrationBuilder.RenameColumn(
                name: "CurrencySymbol",
                table: "Currencies",
                newName: "CssImagePath");
        }
    }
}
