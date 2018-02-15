using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace dal.Migrations
{
    public partial class foreignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Currencies_CurrencyID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SourceAccountID",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_TargetAccountID",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "TargetAccountID",
                table: "Transactions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currencies_CurrencyID",
                table: "Accounts",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SourceAccountID",
                table: "Transactions",
                column: "SourceAccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_TargetAccountID",
                table: "Transactions",
                column: "TargetAccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Currencies_CurrencyID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SourceAccountID",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_TargetAccountID",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "TargetAccountID",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currencies_CurrencyID",
                table: "Accounts",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SourceAccountID",
                table: "Transactions",
                column: "SourceAccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_TargetAccountID",
                table: "Transactions",
                column: "TargetAccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
