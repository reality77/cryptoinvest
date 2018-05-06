using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace dal.Migrations
{
    public partial class AccountPlatform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Platforms_PlatformID",
                table: "Accounts");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Platforms_PlatformID",
                table: "Accounts",
                column: "PlatformID",
                principalTable: "Platforms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
