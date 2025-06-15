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
    }
}