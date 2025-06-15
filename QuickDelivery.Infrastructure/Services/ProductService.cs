using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Database;
using Microsoft.EntityFrameworkCore;

namespace QuickDelivery.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return null;
            }

            return new ProductDto
            {
                Id = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(product => new ProductDto
            {
                Id = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            });
        }

        // NEW methods for many to many relationship

        /// <summary>
        /// Gets all products with their associated categories (Many-to-Many)
        /// This is the main endpoint required by the assignment
        /// </summary>
        public async Task<IEnumerable<ProductWithCategoriesDto>> GetProductsWithCategoriesAsync()
        {
            var products = await _context.Products
                .Include(p => p.Categories) // Include the many-to-many related categories
                .Include(p => p.Partner)    // Include partner information
                .ToListAsync();

            // Process/map the data as required by assignment
            return products.Select(product => MapToProductWithCategoriesDto(product));
        }

        /// <summary>
        /// Gets a specific product with its categories by ID
        /// </summary>
        public async Task<ProductWithCategoriesDto?> GetProductWithCategoriesByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            // Process/map the data
            return MapToProductWithCategoriesDto(product);
        }

        /// <summary>
        /// Gets products by category (Many-to-Many query)
        /// </summary>
        public async Task<IEnumerable<ProductWithCategoriesDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Partner)
                .Where(p => p.Categories.Any(c => c.CategoryId == categoryId))
                .ToListAsync();

            // Process/map the data
            return products.Select(product => MapToProductWithCategoriesDto(product));
        }

        /// <summary>
        /// Private method for mapping entities to DTOs (data processing as required by assignment)
        /// </summary>
        private static ProductWithCategoriesDto MapToProductWithCategoriesDto(Product product)
        {
            return new ProductWithCategoriesDto
            {
                Id = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity,
                PartnerName = product.Partner?.BusinessName ?? "Unknown Partner",
                CreatedAt = product.CreatedAt,
                Categories = product.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl,
                    IsActive = c.IsActive
                }).ToList()
            };
        }
    }
}