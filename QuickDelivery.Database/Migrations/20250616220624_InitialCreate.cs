using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuickDelivery.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TotalOrders = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.PartnerId);
                    table.ForeignKey(
                        name: "FK_Partners_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    DeliveryAddressId = table.Column<int>(type: "int", nullable: false),
                    PickupAddressId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstimatedDeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_PickupAddressId",
                        column: x => x.PickupAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    DelivererId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelivererRating = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DelivererReview = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CurrentLatitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    CurrentLongitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    LastLocationUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryId);
                    table.ForeignKey(
                        name: "FK_Deliveries_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_Users_DelivererId",
                        column: x => x.DelivererId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PaymentGateway = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductsProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.CategoriesCategoryId, x.ProductsProductId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductsProductId",
                        column: x => x.ProductsProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "Description", "IconUrl", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quick meals and snacks", null, true, "Fast Food" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Various pizza types", null, true, "Pizza" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Asian cuisine", null, true, "Asian Food" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sweet treats", null, true, "Desserts" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FirstName", "IsActive", "IsEmailVerified", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "ProfileImageUrl", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@quickdelivery.com", "Admin", true, true, null, "QuickDelivery", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456789", null, 4, null, "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "partner@restaurant.com", "Restaurant", true, true, null, "Owner", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456790", null, 3, null, "restaurant_owner" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john.doe@email.com", "John", true, true, null, "Doe", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456791", null, 1, null, "john_doe" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jane.smith@email.com", "Jane", true, false, null, "Smith", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456792", null, 1, null, "jane_smith" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mike.deliverer@email.com", "Mike", true, true, null, "Johnson", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456793", null, 2, null, "mike_deliverer" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sarah.deliverer@email.com", "Sarah", true, true, null, "Wilson", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456794", null, 2, null, "sarah_deliverer" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@quickdelivery.com", "David", true, true, null, "Manager", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456795", null, 5, null, "david_manager" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "inactive@email.com", "Inactive", false, true, null, "User", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456796", null, 1, null, "inactive_user" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pizza.owner@restaurant.com", "Pizza", true, true, null, "Owner", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456797", null, 3, null, "pizza_owner" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "maria.vip@email.com", "Maria", true, true, new DateTime(2025, 6, 15, 22, 6, 23, 447, DateTimeKind.Utc).AddTicks(3219), "Rodriguez", "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO", "+40123456798", null, 1, null, "maria_vip" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "City", "Country", "CreatedAt", "Instructions", "IsDefault", "Latitude", "Longitude", "PostalCode", "Street", "UserId" },
                values: new object[,]
                {
                    { 1, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "100001", "Strada Principala 1", 1 },
                    { 2, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "100002", "Strada Restaurantului 5", 2 },
                    { 3, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apartament 4, et. 2", true, null, null, "100003", "Bulevardul Unirii 15", 3 },
                    { 4, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Birou, et. 3", false, null, null, "100004", "Strada Victoriei 25", 3 },
                    { 5, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bloc A, scara 1, apt. 15", true, null, null, "100005", "Calea Floreasca 100", 4 },
                    { 6, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "100006", "Strada Aviatorilor 8", 5 },
                    { 7, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vila, poarta alba", true, null, null, "100007", "Bulevardul Herastrau 45", 10 },
                    { 8, "Bucuresti", "Romania", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "100008", "Strada Pizzeriei 12", 9 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "City", "Country", "Name", "PostalCode", "UserId" },
                values: new object[,]
                {
                    { 1, "Bulevardul Unirii 15", "Bucuresti", "Romania", "John Doe", "100003", 3 },
                    { 2, "Calea Floreasca 100", "Bucuresti", "Romania", "Jane Smith", "100005", 4 },
                    { 3, "Bulevardul Herastrau 45", "Bucuresti", "Romania", "Maria Rodriguez", "100007", 10 }
                });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "PartnerId", "AddressId", "AverageRating", "BusinessName", "CloseTime", "CreatedAt", "Description", "IsActive", "LogoUrl", "OpenTime", "TotalOrders", "UserId", "Website" },
                values: new object[,]
                {
                    { 1, 2, 4.5m, "Delicious Restaurant", new TimeSpan(0, 22, 0, 0, 0), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Best food in town", true, null, new TimeSpan(0, 8, 0, 0, 0), 0, 2, null },
                    { 2, 8, 4.8m, "Mario's Pizza Palace", new TimeSpan(0, 23, 0, 0, 0), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Authentic Italian pizza made with love", true, "https://example.com/mario-logo.png", new TimeSpan(0, 10, 0, 0, 0), 0, 9, "https://mariospizza.com" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "PartnerId", "Price", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Pizza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic pizza with tomato sauce, mozzarella and basil", null, true, "Margherita Pizza", 1, 25.50m, 50, null },
                    { 2, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Juicy beef burger with cheese", null, true, "Cheeseburger", 1, 18.00m, 30, null },
                    { 3, "Asian Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Traditional Thai noodles with chicken", null, true, "Chicken Pad Thai", 1, 22.00m, 25, null },
                    { 4, "Desserts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rich chocolate cake with cream", null, true, "Chocolate Cake", 1, 15.00m, 20, null },
                    { 5, "Pizza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic pepperoni pizza with mozzarella", null, true, "Pepperoni Pizza", 2, 28.00m, 40, null },
                    { 6, "Pizza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Four seasons pizza with ham, mushrooms, olives and artichokes", null, true, "Quattro Stagioni", 2, 32.00m, 35, null },
                    { 7, "Desserts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Traditional Italian dessert with coffee and mascarpone", null, true, "Tiramisu", 2, 18.00m, 15, null },
                    { 8, "Fast Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Crispy bread with garlic butter and herbs", null, true, "Garlic Bread", 2, 12.00m, 60, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DelivererId",
                table: "Deliveries",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PartnerId",
                table: "Orders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PickupAddressId",
                table: "Orders",
                column: "PickupAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_AddressId",
                table: "Partners",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_UserId",
                table: "Partners",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductsProductId",
                table: "ProductCategories",
                column: "ProductsProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PartnerId",
                table: "Products",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
