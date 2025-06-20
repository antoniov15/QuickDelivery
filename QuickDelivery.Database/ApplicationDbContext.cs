using Microsoft.EntityFrameworkCore;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Order configurations
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();

                entity.HasOne(o => o.Customer)
                    .WithMany(u => u.OrdersAsCustomer)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Partner)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(o => o.PartnerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.DeliveryAddress)
                    .WithMany()
                    .HasForeignKey(o => o.DeliveryAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.PickupAddress)
                    .WithMany()
                    .HasForeignKey(o => o.PickupAddressId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Delivery configurations
            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithOne(o => o.Delivery)
                    .HasForeignKey<Delivery>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Deliverer)
                    .WithMany(u => u.DeliveriesAsDeliverer)
                    .HasForeignKey(d => d.DelivererId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Partner configurations
            modelBuilder.Entity<Partner>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithOne(u => u.Partner)
                    .HasForeignKey<Partner>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Address)
                    .WithMany()
                    .HasForeignKey(p => p.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer configurations - FIX pentru relația User-Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Customer)
                    .HasForeignKey<Customer>(c => c.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Product configurations
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Partner)
                    .WithMany(pa => pa.Products)
                    .HasForeignKey(p => p.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem configurations
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Address configurations
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Payment configurations
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MANY-TO-MANY CONFIGURATION BETWEEN PRODUCT AND CATEGORY
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategories", // Numele tabelului junction
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoriesCategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductsProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            // Call seed data method
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var staticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var staticRecentLogin = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc); // FIXED: Static value instead of DateTime.UtcNow.AddDays(-1)
            var staticPasswordHash = "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO";

            // 1. USERS
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FirstName = "Admin",
                    LastName = "QuickDelivery",
                    Username = "admin",
                    Email = "admin@quickdelivery.com",
                    PhoneNumber = "+40123456789",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Admin,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },
                new User
                {
                    UserId = 2,
                    FirstName = "Restaurant",
                    LastName = "Owner",
                    Username = "restaurant_owner",
                    Email = "partner@restaurant.com",
                    PhoneNumber = "+40123456790",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Partner,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Customer 1 -  active
                new User
                {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@email.com",
                    Username = "john_doe",
                    PhoneNumber = "+40123456791",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailVerified = true, // both active and verified
                    CreatedAt = staticDate
                },

                // Customer 2 -  Active but unverified email
                new User
                {
                    UserId = 4,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@email.com",
                    Username = "jane_smith",
                    PhoneNumber = "+40123456792",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = true, // active user
                    IsEmailVerified = false, // Unverified email
                    CreatedAt = staticDate
                },

                // Deliverer 1 - Active
                new User
                {
                    UserId = 5,
                    FirstName = "Mike",
                    LastName = "Johnson",
                    Email = "mike.deliverer@email.com",
                    Username = "mike_deliverer",
                    PhoneNumber = "+40123456793",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Deliverer,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Deliverer 2 - Active
                new User
                {
                    UserId = 6,
                    FirstName = "Sarah",
                    LastName = "Wilson",
                    Email = "sarah.deliverer@email.com",
                    Username = "sarah_deliverer",
                    PhoneNumber = "+40123456794",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Deliverer,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Manager
                new User
                {
                    UserId = 7,
                    FirstName = "David",
                    LastName = "Manager",
                    Email = "manager@quickdelivery.com",
                    Username = "david_manager",
                    PhoneNumber = "+40123456795",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Manager,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Inactive Customer
                new User
                {
                    UserId = 8,
                    FirstName = "Inactive",
                    LastName = "User",
                    Email = "inactive@email.com",
                    Username = "inactive_user",
                    PhoneNumber = "+40123456796",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = false, // inactiv
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Second Partner
                new User
                {
                    UserId = 9,
                    FirstName = "Pizza",
                    LastName = "Owner",
                    Email = "pizza.owner@restaurant.com",
                    Username = "pizza_owner",
                    PhoneNumber = "+40123456797",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Partner,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // VIP Customer
                new User
                {
                    UserId = 10,
                    FirstName = "Maria",
                    LastName = "Rodriguez",
                    Email = "maria.vip@email.com",
                    Username = "maria_vip",
                    PhoneNumber = "+40123456798",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate,
                    LastLoginAt = staticRecentLogin 
                },

                // Third partner - inactive
                new User
                {
                    UserId = 11,
                    FirstName = "Closed",
                    LastName = "Restaurant",
                    Email = "closed.restaurant@email.com",
                    Username = "closed_restaurant",
                    PhoneNumber = "+40123456799",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Partner,
                    IsActive = false, // Inactive partner
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Customer 3
                new User
                {
                    UserId = 12,
                    FirstName = "Alex",
                    LastName = "Brown",
                    Email = "alex.brown@email.com",
                    Username = "alex_brown",
                    PhoneNumber = "+40123456800",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Customer 4 - client frecvent
                new User
                {
                    UserId = 13,
                    FirstName = "Emma",
                    LastName = "Davis",
                    Email = "emma.davis@email.com",
                    Username = "emma_davis",
                    PhoneNumber = "+40123456801",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate,
                    LastLoginAt = staticRecentLogin
                },

                // Deliverer 3 - inactive deliverer
                new User
                {
                    UserId = 14,
                    FirstName = "Tom",
                    LastName = "Wilson",
                    Email = "tom.inactive@email.com",
                    Username = "tom_inactive",
                    PhoneNumber = "+40123456802",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Deliverer,
                    IsActive = false, // Inactive deliverer
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                }
            );

            // seeding pentru Customer entity
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    UserId = 3, // John Doe
                    Name = "John Doe",
                    Address = "Bulevardul Unirii 15",
                    City = "Bucuresti",
                    PostalCode = "100003",
                    Country = "Romania"
                },
                new Customer
                {
                    CustomerId = 2,
                    UserId = 4, // Jane Smith
                    Name = "Jane Smith",
                    Address = "Calea Floreasca 100",
                    City = "Bucuresti",
                    PostalCode = "100005",
                    Country = "Romania"
                },
                new Customer
                {
                    CustomerId = 3,
                    UserId = 10, // Maria VIP
                    Name = "Maria Rodriguez",
                    Address = "Bulevardul Herastrau 45",
                    City = "Bucuresti",
                    PostalCode = "100007",
                    Country = "Romania"
                },
                new Customer
                {
                    CustomerId = 4,
                    UserId = 12, // Alex Brown
                    Name = "Alex Brown",
                    Address = "Strada Libertății 33",
                    City = "Bucuresti",
                    PostalCode = "100009",
                    Country = "Romania"
                },
                new Customer
                {
                    CustomerId = 5,
                    UserId = 13, // Emma Davis
                    Name = "Emma Davis",
                    Address = "Calea Victoriei 150",
                    City = "Bucuresti",
                    PostalCode = "100010",
                    Country = "Romania"
                }
            );

            // 2. ADDRESSES
            modelBuilder.Entity<Address>().HasData(
                // Admin address
                new Address
                {
                    AddressId = 1,
                    UserId = 1,
                    Street = "Strada Principala 1",
                    City = "Bucuresti",
                    PostalCode = "100001",
                    Country = "Romania",
                    IsDefault = true,
                    CreatedAt = staticDate
                },

                // Restaurant address
                new Address
                {
                    AddressId = 2,
                    UserId = 2,
                    Street = "Strada Restaurantului 5",
                    City = "Bucuresti",
                    PostalCode = "100002",
                    Country = "Romania",
                    IsDefault = true,
                    CreatedAt = staticDate
                },

                // John Doe addresses
                new Address
                {
                    AddressId = 3,
                    UserId = 3,
                    Street = "Bulevardul Unirii 15",
                    City = "Bucuresti",
                    PostalCode = "100003",
                    Country = "Romania",
                    IsDefault = true,
                    Instructions = "Apartament 4, et. 2",
                    CreatedAt = staticDate
                },

                new Address
                {
                    AddressId = 4,
                    UserId = 3,
                    Street = "Strada Victoriei 25",
                    City = "Bucuresti",
                    PostalCode = "100004",
                    Country = "Romania",
                    IsDefault = false,
                    Instructions = "Birou, et. 3",
                    CreatedAt = staticDate
                },

                // Jane Smith address
                new Address
                {
                    AddressId = 5,
                    UserId = 4,
                    Street = "Calea Floreasca 100",
                    City = "Bucuresti",
                    PostalCode = "100005",
                    Country = "Romania",
                    IsDefault = true,
                    Instructions = "Bloc A, scara 1, apt. 15",
                    CreatedAt = staticDate
                },

                // Mike Deliverer address
                new Address
                {
                    AddressId = 6,
                    UserId = 5,
                    Street = "Strada Aviatorilor 8",
                    City = "Bucuresti",
                    PostalCode = "100006",
                    Country = "Romania",
                    IsDefault = true,
                    CreatedAt = staticDate
                },

                // Maria VIP address
                new Address
                {
                    AddressId = 7,
                    UserId = 10,
                    Street = "Bulevardul Herastrau 45",
                    City = "Bucuresti",
                    PostalCode = "100007",
                    Country = "Romania",
                    IsDefault = true,
                    Instructions = "Vila, poarta alba",
                    CreatedAt = staticDate
                },

                // Second restaurant address
                new Address
                {
                    AddressId = 8,
                    UserId = 9,
                    Street = "Strada Pizzeriei 12",
                    City = "Bucuresti",
                    PostalCode = "100008",
                    Country = "Romania",
                    IsDefault = true,
                    CreatedAt = staticDate
                },
                new Address
                {
                    AddressId = 9,
                    UserId = 12, // Alex Brown
                    Street = "Strada Libertății 33",
                    City = "Bucuresti",
                    PostalCode = "100009",
                    Country = "Romania",
                    IsDefault = true,
                    Instructions = "Apartament 12",
                    CreatedAt = staticDate
                },
                new Address
                {
                    AddressId = 10,
                    UserId = 13, // Emma Davis
                    Street = "Calea Victoriei 150",
                    City = "Bucuresti",
                    PostalCode = "100010",
                    Country = "Romania",
                    IsDefault = true,
                    Instructions = "Casa cu gard verde",
                    CreatedAt = staticDate
                },
                new Address
                {
                    AddressId = 11,
                    UserId = 11, // Closed restaurant
                    Street = "Strada Închisă 99",
                    City = "Bucuresti",
                    PostalCode = "100011",
                    Country = "Romania",
                    IsDefault = true,
                    CreatedAt = staticDate
                }
            );

            // 3. PARTNERS
            modelBuilder.Entity<Partner>().HasData(
                new Partner
                {
                    PartnerId = 1,
                    UserId = 2,
                    BusinessName = "Delicious Restaurant",
                    Description = "Best food in town",
                    AddressId = 2,
                    OpenTime = new TimeSpan(8, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0),
                    AverageRating = 4.5m,
                    TotalOrders = 0,
                    IsActive = true,
                    CreatedAt = staticDate
                },

                // New pizza restaurant
                new Partner
                {
                    PartnerId = 2,
                    UserId = 9,
                    BusinessName = "Mario's Pizza Palace",
                    Description = "Authentic Italian pizza made with love",
                    Website = "https://mariospizza.com",
                    LogoUrl = "https://example.com/mario-logo.png",
                    AddressId = 8,
                    OpenTime = new TimeSpan(10, 0, 0),
                    CloseTime = new TimeSpan(23, 0, 0),
                    AverageRating = 4.8m,
                    TotalOrders = 0,
                    IsActive = true,
                    CreatedAt = staticDate
                },

                // partener inactiv
                new Partner
                {
                    PartnerId = 3,
                    UserId = 11,
                    BusinessName = "Closed Bistro",
                    Description = "Currently closed for renovations",
                    AddressId = 11,
                    OpenTime = new TimeSpan(9, 0, 0),
                    CloseTime = new TimeSpan(21, 0, 0),
                    AverageRating = 3.2m,
                    TotalOrders = 5,
                    IsActive = false, // Inactive partner
                    CreatedAt = staticDate
                }
            );

            // 4. CATEGORIES - ONLY HERE, NO DUPLICATES
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Fast Food",
                    Description = "Quick meals and snacks",
                    IsActive = true,
                    CreatedAt = staticDate
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Pizza",
                    Description = "Various pizza types",
                    IsActive = true,
                    CreatedAt = staticDate
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Asian Food",
                    Description = "Asian cuisine",
                    IsActive = true,
                    CreatedAt = staticDate
                },
                new Category
                {
                    CategoryId = 4,
                    Name = "Desserts",
                    Description = "Sweet treats",
                    IsActive = true,
                    CreatedAt = staticDate
                }
            );

            // 5. PRODUCTS
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    PartnerId = 1,
                    Name = "Margherita Pizza",
                    Description = "Classic pizza with tomato sauce, mozzarella and basil",
                    Price = 25.50m,
                    Category = "Pizza",
                    IsAvailable = true,
                    StockQuantity = 50,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 2,
                    PartnerId = 1,
                    Name = "Cheeseburger",
                    Description = "Juicy beef burger with cheese",
                    Price = 18.00m,
                    Category = "Fast Food",
                    IsAvailable = true,
                    StockQuantity = 30,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 3,
                    PartnerId = 1,
                    Name = "Chicken Pad Thai",
                    Description = "Traditional Thai noodles with chicken",
                    Price = 22.00m,
                    Category = "Asian Food",
                    IsAvailable = true,
                    StockQuantity = 25,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 4,
                    PartnerId = 1,
                    Name = "Chocolate Cake",
                    Description = "Rich chocolate cake with cream",
                    Price = 15.00m,
                    Category = "Desserts",
                    IsAvailable = true,
                    StockQuantity = 20,
                    CreatedAt = staticDate
                },

                // Products from Mario's Pizza Palace (Partner 2)
                new Product
                {
                    ProductId = 5,
                    PartnerId = 2,
                    Name = "Pepperoni Pizza",
                    Description = "Classic pepperoni pizza with mozzarella",
                    Price = 28.00m,
                    Category = "Pizza",
                    IsAvailable = true,
                    StockQuantity = 40,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 6,
                    PartnerId = 2,
                    Name = "Quattro Stagioni",
                    Description = "Four seasons pizza with ham, mushrooms, olives and artichokes",
                    Price = 32.00m,
                    Category = "Pizza",
                    IsAvailable = true,
                    StockQuantity = 35,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 7,
                    PartnerId = 2,
                    Name = "Tiramisu",
                    Description = "Traditional Italian dessert with coffee and mascarpone",
                    Price = 18.00m,
                    Category = "Desserts",
                    IsAvailable = true,
                    StockQuantity = 15,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 8,
                    PartnerId = 2,
                    Name = "Garlic Bread",
                    Description = "Crispy bread with garlic butter and herbs",
                    Price = 12.00m,
                    Category = "Fast Food",
                    IsAvailable = true,
                    StockQuantity = 60,
                    CreatedAt = staticDate
                },
                // Out of stock products
                new Product
                {
                    ProductId = 9,
                    PartnerId = 1,
                    Name = "Sold Out Pasta",
                    Description = "Delicious pasta that's currently sold out",
                    Price = 19.50m,
                    Category = "Fast Food",
                    IsAvailable = true,
                    StockQuantity = 0, // OUT OF STOCK
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 10,
                    PartnerId = 2,
                    Name = "Special Pizza",
                    Description = "Limited edition pizza - currently out of stock",
                    Price = 35.00m,
                    Category = "Pizza",
                    IsAvailable = true,
                    StockQuantity = 0, // OUT OF STOCK
                    CreatedAt = staticDate
                },
                // Unavailable products
                new Product
                {
                    ProductId = 11,
                    PartnerId = 1,
                    Name = "Seasonal Soup",
                    Description = "Only available in winter",
                    Price = 14.00m,
                    Category = "Fast Food",
                    IsAvailable = false, // NOT AVAILABLE
                    StockQuantity = 25,
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 12,
                    PartnerId = 2,
                    Name = "Premium Dessert",
                    Description = "Temporarily removed from menu",
                    Price = 25.00m,
                    Category = "Desserts",
                    IsAvailable = false, // NOT AVAILABLE
                    StockQuantity = 10,
                    CreatedAt = staticDate
                },
                // Low stock products
                new Product
                {
                    ProductId = 13,
                    PartnerId = 1,
                    Name = "Last Chance Burger",
                    Description = "Almost sold out - only a few left",
                    Price = 21.00m,
                    Category = "Fast Food",
                    IsAvailable = true,
                    StockQuantity = 2, // LOW STOCK
                    CreatedAt = staticDate
                },
                new Product
                {
                    ProductId = 14,
                    PartnerId = 2,
                    Name = "Final Slice Pizza",
                    Description = "Last pieces available",
                    Price = 29.00m,
                    Category = "Pizza",
                    IsAvailable = true,
                    StockQuantity = 1, // VERY LOW STOCK
                    CreatedAt = staticDate
                },
                // Products from inactive partner
                new Product
                {
                    ProductId = 15,
                    PartnerId = 3, // Closed Bistro
                    Name = "Closed Special",
                    Description = "From closed restaurant",
                    Price = 16.00m,
                    Category = "Fast Food",
                    IsAvailable = false,
                    StockQuantity = 0,
                    CreatedAt = staticDate
                }
            );

            // 6. ORDERS
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    OrderId = 1,
                    CustomerId = 1, // John Doe
                    PartnerId = 1,  // Delicious Restaurant
                    OrderNumber = "ORD-2025-001",
                    Status = OrderStatus.Delivered,
                    TotalAmount = 43.50m,
                    DeliveryAddressId = 3,
                    PickupAddressId = 2,
                    CreatedAt = staticDate
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = 2, // Jane Smith
                    PartnerId = 2,  // Mario's Pizza Palace
                    OrderNumber = "ORD-2025-002",
                    Status = OrderStatus.Pending,
                    TotalAmount = 60.00m,
                    DeliveryAddressId = 5,
                    PickupAddressId = 8,
                    CreatedAt = staticDate.AddDays(1)
                },
                // Confirmed order
                new Order
                {
                    OrderId = 3,
                    CustomerId = 3, // Maria VIP
                    PartnerId = 1,
                    OrderNumber = "ORD-20250003",
                    Status = OrderStatus.Confirmed,
                    TotalAmount = 37.50m,
                    SubTotal = 32.50m,
                    DeliveryFee = 5.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 7,
                    PickupAddressId = 2,
                    CreatedAt = staticDate.AddDays(2),
                    EstimatedDeliveryTime = staticDate.AddDays(2).AddMinutes(40),
                    Notes = "VIP customer - priority handling"
                },
                // Preparing order
                new Order
                {
                    OrderId = 4,
                    CustomerId = 4, // Alex Brown
                    PartnerId = 2,
                    OrderNumber = "ORD-20250004",
                    Status = OrderStatus.Preparing,
                    TotalAmount = 50.00m,
                    SubTotal = 45.00m,
                    DeliveryFee = 5.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 9,
                    PickupAddressId = 8,
                    CreatedAt = staticDate.AddDays(3),
                    EstimatedDeliveryTime = staticDate.AddDays(3).AddMinutes(35),
                    SpecialInstructions = "Extra spicy"
                },
                // Ready for pickup
                new Order
                {
                    OrderId = 5,
                    CustomerId = 5, // Emma Davis
                    PartnerId = 1,
                    OrderNumber = "ORD-20250005",
                    Status = OrderStatus.ReadyForPickup,
                    TotalAmount = 29.50m,
                    SubTotal = 25.50m,
                    DeliveryFee = 4.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 10,
                    PickupAddressId = 2,
                    CreatedAt = staticDate.AddDays(4),
                    EstimatedDeliveryTime = staticDate.AddDays(4).AddMinutes(25)
                },
                // In transit
                new Order
                {
                    OrderId = 6,
                    CustomerId = 1, // John Doe (repeat customer)
                    PartnerId = 2,
                    OrderNumber = "ORD-20250006",
                    Status = OrderStatus.InTransit,
                    TotalAmount = 45.00m,
                    SubTotal = 40.00m,
                    DeliveryFee = 5.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 4, // Work address
                    PickupAddressId = 8,
                    CreatedAt = staticDate.AddDays(5),
                    EstimatedDeliveryTime = staticDate.AddDays(5).AddMinutes(30),
                    Notes = "Delivery to office building"
                },
                // Cancelled order
                new Order
                {
                    OrderId = 7,
                    CustomerId = 2, // Jane Smith
                    PartnerId = 1,
                    OrderNumber = "ORD-20250007",
                    Status = OrderStatus.Cancelled,
                    TotalAmount = 33.00m,
                    SubTotal = 28.00m,
                    DeliveryFee = 5.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 5,
                    PickupAddressId = 2,
                    CreatedAt = staticDate.AddDays(6),
                    Notes = "Customer cancelled - out of stock item"
                },
                // Refunded order
                new Order
                {
                    OrderId = 8,
                    CustomerId = 3, // Maria VIP
                    PartnerId = 2,
                    OrderNumber = "ORD-20250008",
                    Status = OrderStatus.Refunded,
                    TotalAmount = 52.00m,
                    SubTotal = 47.00m,
                    DeliveryFee = 5.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 7,
                    PickupAddressId = 8,
                    CreatedAt = staticDate.AddDays(7),
                    ActualDeliveryTime = staticDate.AddDays(7).AddHours(1),
                    Notes = "Delivered but customer was not satisfied - full refund issued"
                },
                // Another pending order for testing
                new Order
                {
                    OrderId = 9,
                    CustomerId = 4, // Alex Brown
                    PartnerId = 1,
                    OrderNumber = "ORD-20250009",
                    Status = OrderStatus.Pending,
                    TotalAmount = 21.00m,
                    SubTotal = 18.00m,
                    DeliveryFee = 3.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 9,
                    PickupAddressId = 2,
                    CreatedAt = staticDate.AddDays(8),
                    EstimatedDeliveryTime = staticDate.AddDays(8).AddMinutes(20)
                },
                // Large order - delivered
                new Order
                {
                    OrderId = 10,
                    CustomerId = 5, // Emma Davis
                    PartnerId = 2,
                    OrderNumber = "ORD-20250010",
                    Status = OrderStatus.Delivered,
                    TotalAmount = 98.50m,
                    SubTotal = 89.50m,
                    DeliveryFee = 9.00m,
                    Tax = 0m,
                    Discount = 0m,
                    DeliveryAddressId = 10,
                    PickupAddressId = 8,
                    CreatedAt = staticDate.AddDays(9),
                    ActualDeliveryTime = staticDate.AddDays(9).AddMinutes(50),
                    EstimatedDeliveryTime = staticDate.AddDays(9).AddMinutes(45),
                    Notes = "Large family order",
                    SpecialInstructions = "Ring doorbell twice"
                }
            );

            // 7. ORDER ITEMS
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    OrderItemId = 1,
                    OrderId = 1,
                    ProductId = 1, // Margherita Pizza
                    Quantity = 1,
                    UnitPrice = 25.50m,
                    TotalPrice = 25.50m
                },
                new OrderItem
                {
                    OrderItemId = 2,
                    OrderId = 1,
                    ProductId = 2, // Cheeseburger
                    Quantity = 1,
                    UnitPrice = 18.00m,
                    TotalPrice = 18.00m
                },
                new OrderItem
                {
                    OrderItemId = 3,
                    OrderId = 2,
                    ProductId = 5, // Pepperoni Pizza
                    Quantity = 2,
                    UnitPrice = 28.00m,
                    TotalPrice = 56.00m
                },
                // Order 3
                new OrderItem
                {
                    OrderItemId = 4,
                    OrderId = 3,
                    ProductId = 4, // Chocolate Cake
                    Quantity = 1,
                    UnitPrice = 15.00m,
                    TotalPrice = 15.00m
                },
                new OrderItem
                {
                    OrderItemId = 5,
                    OrderId = 3,
                    ProductId = 7, // Tiramisu
                    Quantity = 1,
                    UnitPrice = 18.00m,
                    TotalPrice = 18.00m
                },
                // Order 4 items
                new OrderItem
                {
                    OrderItemId = 6,
                    OrderId = 4,
                    ProductId = 6, // Quattro Stagioni
                    Quantity = 1,
                    UnitPrice = 32.00m,
                    TotalPrice = 32.00m
                },
                new OrderItem
                {
                    OrderItemId = 7,
                    OrderId = 4,
                    ProductId = 8, // Garlic Bread
                    Quantity = 1,
                    UnitPrice = 12.00m,
                    TotalPrice = 12.00m
                },
                // Order 5 items
                new OrderItem
                {
                    OrderItemId = 8,
                    OrderId = 5,
                    ProductId = 1, // Margherita Pizza
                    Quantity = 1,
                    UnitPrice = 25.50m,
                    TotalPrice = 25.50m
                },
                // Order 6 items
                new OrderItem
                {
                    OrderItemId = 9,
                    OrderId = 6,
                    ProductId = 5, // Pepperoni Pizza
                    Quantity = 1,
                    UnitPrice = 28.00m,
                    TotalPrice = 28.00m
                },
                new OrderItem
                {
                    OrderItemId = 10,
                    OrderId = 6,
                    ProductId = 8, // Garlic Bread
                    Quantity = 1,
                    UnitPrice = 12.00m,
                    TotalPrice = 12.00m
                },
                // Order 7 items (cancelled)
                new OrderItem
                {
                    OrderItemId = 11,
                    OrderId = 7,
                    ProductId = 3, // Chicken Pad Thai
                    Quantity = 1,
                    UnitPrice = 22.00m,
                    TotalPrice = 22.00m,
                    SpecialInstructions = "No spicy"
                },
                new OrderItem
                {
                    OrderItemId = 12,
                    OrderId = 7,
                    ProductId = 4, // Chocolate Cake
                    Quantity = 1,
                    UnitPrice = 15.00m,
                    TotalPrice = 15.00m
                },
                // Order 8 items (refunded)
                new OrderItem
                {
                    OrderItemId = 13,
                    OrderId = 8,
                    ProductId = 6, // Quattro Stagioni
                    Quantity = 1,
                    UnitPrice = 32.00m,
                    TotalPrice = 32.00m
                },
                new OrderItem
                {
                    OrderItemId = 14,
                    OrderId = 8,
                    ProductId = 7, // Tiramisu
                    Quantity = 1,
                    UnitPrice = 18.00m,
                    TotalPrice = 18.00m
                },
                // Order 9 items
                new OrderItem
                {
                    OrderItemId = 15,
                    OrderId = 9,
                    ProductId = 2, // Cheeseburger
                    Quantity = 1,
                    UnitPrice = 18.00m,
                    TotalPrice = 18.00m,
                    SpecialInstructions = "Extra cheese"
                },
                // Order 10 items (large order)
                new OrderItem
                {
                    OrderItemId = 16,
                    OrderId = 10,
                    ProductId = 5, // Pepperoni Pizza
                    Quantity = 2,
                    UnitPrice = 28.00m,
                    TotalPrice = 56.00m
                },
                new OrderItem
                {
                    OrderItemId = 17,
                    OrderId = 10,
                    ProductId = 6, // Quattro Stagioni
                    Quantity = 1,
                    UnitPrice = 32.00m,
                    TotalPrice = 32.00m
                },
                new OrderItem
                {
                    OrderItemId = 18,
                    OrderId = 10,
                    ProductId = 8, // Garlic Bread
                    Quantity = 1,
                    UnitPrice = 12.00m,
                    TotalPrice = 12.00m
                }
            );
        }
    }
}