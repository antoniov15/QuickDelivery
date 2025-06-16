using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Database;
using Microsoft.EntityFrameworkCore;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Products;
using System.Linq;

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

        // OLD methods for backwards compatibility

        /// <summary>
        /// Gets all products with their associated categories (Many-to-Many) - Original Method
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

        // NEW -- Advanced filtering, pagination and sorting

        /// <summary>
        /// Gets products with categories using advanced filtering, pagination, and sorting
        /// </summary>
        public async Task<PaginatedResult<ProductWithCategoriesDto>> GetProductsWithCategoriesAsync(ProductQueryParameters parameters)
        {
            var query = _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Partner)
                .AsQueryable();

            // FILTERING 
            // by search term (name or description)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
            }

            // by price range
            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= parameters.MaxPrice.Value);
            }

            // by availability
            if (parameters.IsAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == parameters.IsAvailable.Value);
            }

            // by category name
            if (!string.IsNullOrWhiteSpace(parameters.CategoryName))
            {
                query = query.Where(p => p.Categories.Any(c =>
                    c.Name.ToLower().Contains(parameters.CategoryName.ToLower())));
            }

            // by partner name
            if (!string.IsNullOrWhiteSpace(parameters.PartnerName))
            {
                query = query.Where(p => p.Partner != null &&
                    p.Partner.BusinessName.ToLower().Contains(parameters.PartnerName.ToLower()));
            }

            // SORTING
            query = parameters.SortBy.ToLower() switch
            {
                "name" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "price" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "createdat" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),

                "stockquantity" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.StockQuantity)
                    : query.OrderBy(p => p.StockQuantity),

                _ => query.OrderBy(p => p.Name) // Default sorting by name
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // PAGINATION
            var products = await query
                .Skip(parameters.Skip)
                .Take(parameters.Take)
                .ToListAsync();

            // Map to DTOs
            var productDtos = products.Select(product => MapToProductWithCategoriesDto(product));

            // Return paginated result
            return PaginatedResult<ProductWithCategoriesDto>.Create(
                productDtos,
                totalCount,
                parameters.Page,
                parameters.PageSize
            );
        }

        // OLD methods for backwards compatibility in continuare

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