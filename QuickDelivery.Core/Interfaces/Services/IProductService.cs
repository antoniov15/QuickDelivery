using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Products;

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

        // advanced filtering, pagination and sorting
        Task<PaginatedResult<ProductWithCategoriesDto>> GetProductsWithCategoriesAsync(ProductQueryParameters parameters);

        Task<ProductWithCategoriesDto?> UpdateProductAsync(int productId, UpdateProductDto updateProductDto);
        Task<ProductWithCategoriesDto?> PartialUpdateProductAsync(int productId, UpdateProductDto updateProductDto);
    }
}