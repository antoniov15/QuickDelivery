using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        // Many-to-Many relationship
        Task<IEnumerable<ProductWithCategoriesDto>> GetProductsWithCategoriesAsync();
        Task<ProductWithCategoriesDto?> GetProductWithCategoriesByIdAsync(int productId);
        Task<IEnumerable<ProductWithCategoriesDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
