using QuickDelivery.Core.DTOs;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        // Many-to-Many relationship methods
        Task<IEnumerable<ProductWithCategoriesDto>> GetProductsWithCategoriesAsync();
        Task<ProductWithCategoriesDto?> GetProductWithCategoriesByIdAsync(int productId);
        Task<IEnumerable<ProductWithCategoriesDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
