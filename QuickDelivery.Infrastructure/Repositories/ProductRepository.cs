using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Database;
using Microsoft.EntityFrameworkCore;

namespace QuickDelivery.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Implementăm metoda GetByIdAsync
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Partner)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // Implementăm metoda UpdateAsync
        public async Task<Product> UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }
    }
}