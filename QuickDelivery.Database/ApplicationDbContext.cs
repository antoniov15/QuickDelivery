using Microsoft.EntityFrameworkCore;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

            // Many-to-many configuration între Product și Category
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity(j => j.ToTable("ProductCategories")); // Numele tabelei de legătură

            // Seed data pentru Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Fast Food", Description = "Quick meals and snacks" },
                new Category { CategoryId = 2, Name = "Pizza", Description = "Various pizza types" },
                new Category { CategoryId = 3, Name = "Asian Food", Description = "Asian cuisine" },
                new Category { CategoryId = 4, Name = "Desserts", Description = "Sweet treats" }
                );
            
            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Date statice pentru a evita eroarea de model dinamic
            var staticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Hash static pre-generat pentru parola "admin123"
            var staticPasswordHash = "$2a$11$bfPciUVybJ3vtJOW.5JvQu6sYqgf1wu76PbwsIlYByyzVTZ6KsJkO";

            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FirstName = "Admin",
                    LastName = "QuickDelivery",
                    Email = "admin@quickdelivery.com",
                    PhoneNumber = "+40123456789",
                    PasswordHash = staticPasswordHash, // Hash static in loc de BCrypt.HashPassword
                    Role = UserRole.Admin,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = staticDate
                }
            );

            // Seed default address for admin
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
                }
            );
        }
    }
}