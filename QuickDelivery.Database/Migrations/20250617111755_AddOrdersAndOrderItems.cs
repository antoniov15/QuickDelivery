using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickDelivery.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "ActualDeliveryTime", "CreatedAt", "CustomerId", "DeliveryAddressId", "DeliveryFee", "Discount", "EstimatedDeliveryTime", "Notes", "OrderNumber", "PartnerId", "PickupAddressId", "SpecialInstructions", "Status", "SubTotal", "Tax", "TotalAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, 0m, 0m, null, null, "ORD-2025-001", 1, 2, null, 6, 0m, 0m, 43.50m, null },
                    { 2, null, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 2, 5, 0m, 0m, null, null, "ORD-2025-002", 2, 8, null, 1, 0m, 0m, 60.00m, null }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "OrderId", "ProductId", "Quantity", "SpecialInstructions", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, null, 25.50m, 25.50m },
                    { 2, 1, 2, 1, null, 18.00m, 18.00m },
                    { 3, 2, 5, 2, null, 56.00m, 28.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2);
        }
    }
}
