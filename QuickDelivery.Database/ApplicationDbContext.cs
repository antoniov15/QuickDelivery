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

                // Customer 1
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
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                },

                // Customer 2
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
                    IsActive = true,
                    IsEmailVerified = false, // Unverified email for testing
                    CreatedAt = staticDate
                },

                // Deliverer 1
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

                // Deliverer 2
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

                // Inactive Customer (for testing)
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
                    IsActive = false, // Inactive for testing
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
                    LastLoginAt = DateTime.UtcNow.AddDays(-1) // Recent login
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
                }
            );
        }
    }
}