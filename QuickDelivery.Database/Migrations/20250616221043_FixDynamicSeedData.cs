using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickDelivery.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "LastLoginAt",
                value: new DateTime(2024, 6, 15, 10, 30, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "LastLoginAt",
                value: new DateTime(2025, 6, 15, 22, 6, 23, 447, DateTimeKind.Utc).AddTicks(3219));
        }
    }
}
