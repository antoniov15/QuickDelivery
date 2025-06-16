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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
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
                .UsingEntity(j => j.ToTable("ProductCategories"));

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
                    Email = "partner@restaurant.com",
                    PhoneNumber = "+40123456790",
                    PasswordHash = staticPasswordHash,
                    Role = UserRole.Partner,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                }
            );

            // 2. ADDRESSES
            modelBuilder.Entity<Address>().HasData(
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
                }
            );
        }
    }
}