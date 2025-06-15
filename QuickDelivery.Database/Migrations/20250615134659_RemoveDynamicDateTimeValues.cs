using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickDelivery.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDynamicDateTimeValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 46, 59, 166, DateTimeKind.Utc).AddTicks(7243));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 46, 59, 166, DateTimeKind.Utc).AddTicks(7794));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 46, 59, 166, DateTimeKind.Utc).AddTicks(7796));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 46, 59, 166, DateTimeKind.Utc).AddTicks(7796));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 21, 59, 898, DateTimeKind.Utc).AddTicks(107));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 21, 59, 898, DateTimeKind.Utc).AddTicks(685));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 21, 59, 898, DateTimeKind.Utc).AddTicks(687));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 15, 13, 21, 59, 898, DateTimeKind.Utc).AddTicks(688));
        }
    }
}
