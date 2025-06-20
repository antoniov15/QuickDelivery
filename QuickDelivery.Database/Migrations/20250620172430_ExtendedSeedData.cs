using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickDelivery.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "ActualDeliveryTime", "CreatedAt", "CustomerId", "DeliveryAddressId", "DeliveryFee", "Discount", "EstimatedDeliveryTime", "Notes", "OrderNumber", "PartnerId", "PickupAddressId", "SpecialInstructions", "Status", "SubTotal", "Tax", "TotalAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 3, null, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 3, 7, 5.00m, 0m, new DateTime(2024, 1, 3, 0, 40, 0, 0, DateTimeKind.Utc), "VIP customer - priority handling", "ORD-20250003", 1, 2, null, 2, 32.50m, 0m, 37.50m, null },
                    { 6, null, new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, 5.00m, 0m, new DateTime(2024, 1, 6, 0, 30, 0, 0, DateTimeKind.Utc), "Delivery to office building", "ORD-20250006", 2, 8, null, 5, 40.00m, 0m, 45.00m, null },
                    { 7, null, new DateTime(2024, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), 2, 5, 5.00m, 0m, null, "Customer cancelled - out of stock item", "ORD-20250007", 1, 2, null, 7, 28.00m, 0m, 33.00m, null },
                    { 8, new DateTime(2024, 1, 8, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 3, 7, 5.00m, 0m, null, "Delivered but customer was not satisfied - full refund issued", "ORD-20250008", 2, 8, null, 8, 47.00m, 0m, 52.00m, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "PartnerId", "Price", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 9, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Delicious pasta that's currently sold out", null, true, "Sold Out Pasta", 1, 19.50m, 0, null },
                    { 10, "Pizza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Limited edition pizza - currently out of stock", null, true, "Special Pizza", 2, 35.00m, 0, null },
                    { 11, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Only available in winter", null, false, "Seasonal Soup", 1, 14.00m, 25, null },
                    { 12, "Desserts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Temporarily removed from menu", null, false, "Premium Dessert", 2, 25.00m, 10, null },
                    { 13, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Almost sold out - only a few left", null, true, "Last Chance Burger", 1, 21.00m, 2, null },
                    { 14, "Pizza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Last pieces available", null, true, "Final Slice Pizza", 2, 29.00m, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FirstName", "IsActive", "IsEmailVerified", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "ProfileImageUrl", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "closed.restaurant@email.com", "Closed", false, true, null, "Restaurant", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456799", null, 3, null, "closed_restaurant" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alex.brown@email.com", "Alex", true, true, null, "Brown", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456800", null, 1, null, "alex_brown" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "emma.davis@email.com", "Emma", true, true, new DateTime(2024, 6, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Davis", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456801", null, 1, null, "emma_davis" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tom.inactive@email.com", "Tom", false, true, null, "Wilson", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456802", null, 2, null, "tom_inactive" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "City", "Country", "CreatedAt", "Instructions", "IsDefault", "Latitude", "Longitude", "PostalCode", "Street", "UserId" },
                values: new object[,]
                {
                    { 9, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apartament 12", true, null, null, "100009", "Strada Libertății 33", 12 },
                    { 10, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Casa cu gard verde", true, null, null, "100010", "Calea Victoriei 150", 13 },
                    { 11, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "100011", "Strada Închisă 99", 11 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "City", "Country", "Name", "PostalCode", "UserId" },
                values: new object[,]
                {
                    { 4, "Strada Libertății 33", "Bucuresti", "Romania", "Alex Brown", "100009", 12 },
                    { 5, "Calea Victoriei 150", "Bucuresti", "Romania", "Emma Davis", "100010", 13 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "OrderId", "ProductId", "Quantity", "SpecialInstructions", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 4, 3, 4, 1, null, 15.00m, 15.00m },
                    { 5, 3, 7, 1, null, 18.00m, 18.00m },
                    { 9, 6, 5, 1, null, 28.00m, 28.00m },
                    { 10, 6, 8, 1, null, 12.00m, 12.00m },
                    { 11, 7, 3, 1, "No spicy", 22.00m, 22.00m },
                    { 12, 7, 4, 1, null, 15.00m, 15.00m },
                    { 13, 8, 6, 1, null, 32.00m, 32.00m },
                    { 14, 8, 7, 1, null, 18.00m, 18.00m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "ActualDeliveryTime", "CreatedAt", "CustomerId", "DeliveryAddressId", "DeliveryFee", "Discount", "EstimatedDeliveryTime", "Notes", "OrderNumber", "PartnerId", "PickupAddressId", "SpecialInstructions", "Status", "SubTotal", "Tax", "TotalAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, null, new DateTime(2024, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), 4, 9, 5.00m, 0m, new DateTime(2024, 1, 4, 0, 35, 0, 0, DateTimeKind.Utc), null, "ORD-20250004", 2, 8, "Extra spicy", 3, 45.00m, 0m, 50.00m, null },
                    { 5, null, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 5, 10, 4.00m, 0m, new DateTime(2024, 1, 5, 0, 25, 0, 0, DateTimeKind.Utc), null, "ORD-20250005", 1, 2, null, 4, 25.50m, 0m, 29.50m, null },
                    { 9, null, new DateTime(2024, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), 4, 9, 3.00m, 0m, new DateTime(2024, 1, 9, 0, 20, 0, 0, DateTimeKind.Utc), null, "ORD-20250009", 1, 2, null, 1, 18.00m, 0m, 21.00m, null },
                    { 10, new DateTime(2024, 1, 10, 0, 50, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), 5, 10, 9.00m, 0m, new DateTime(2024, 1, 10, 0, 45, 0, 0, DateTimeKind.Utc), "Large family order", "ORD-20250010", 2, 8, "Ring doorbell twice", 6, 89.50m, 0m, 98.50m, null }
                });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "PartnerId", "AddressId", "AverageRating", "BusinessName", "CloseTime", "CreatedAt", "Description", "IsActive", "LogoUrl", "OpenTime", "TotalOrders", "UserId", "Website" },
                values: new object[] { 3, 11, 3.2m, "Closed Bistro", new TimeSpan(0, 21, 0, 0, 0), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Currently closed for renovations", false, null, new TimeSpan(0, 9, 0, 0, 0), 5, 11, null });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "OrderId", "ProductId", "Quantity", "SpecialInstructions", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 6, 4, 6, 1, null, 32.00m, 32.00m },
                    { 7, 4, 8, 1, null, 12.00m, 12.00m },
                    { 8, 5, 1, 1, null, 25.50m, 25.50m },
                    { 15, 9, 2, 1, "Extra cheese", 18.00m, 18.00m },
                    { 16, 10, 5, 2, null, 56.00m, 28.00m },
                    { 17, 10, 6, 1, null, 32.00m, 32.00m },
                    { 18, 10, 8, 1, null, 12.00m, 12.00m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "PartnerId", "Price", "StockQuantity", "UpdatedAt" },
                values: new object[] { 15, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "From closed restaurant", null, false, "Closed Special", 3, 16.00m, 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Partners",
                keyColumn: "PartnerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13);
        }
    }
}
