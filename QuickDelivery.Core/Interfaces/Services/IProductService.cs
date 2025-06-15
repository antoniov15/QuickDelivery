using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}
