using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace dal.Migrations
{
    public partial class SourceGrossAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetAmount",
                table: "Transactions",
                newName: "TargetNetAmount");

            migrationBuilder.RenameColumn(
                name: "SourceAmount",
                table: "Transactions",
                newName: "SourceGrossAmount");

            migrationBuilder.Sql(@"UPDATE ""Transactions"" SET ""SourceGrossAmount"" = ""SourceGrossAmount"" + ""SourceFees"";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetNetAmount",
                table: "Transactions",
                newName: "TargetAmount");

            migrationBuilder.RenameColumn(
                name: "SourceGrossAmount",
                table: "Transactions",
                newName: "SourceAmount");
        }
    }
}
