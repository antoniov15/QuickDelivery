using QuickDelivery.Core.Entities;

namespace QuickDelivery.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);

        Task<Product> UpdateAsync(Product product);
    }
}