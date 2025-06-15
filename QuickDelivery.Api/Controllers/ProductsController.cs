using Microsoft.AspNetCore.Mvc;
using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Core.DTOs.Common;
using Microsoft.Extensions.Logging;

namespace QuickDelivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with their associated categories (Many-to-Many relationship)
        /// </summary>
        /// <returns>List of products with categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductWithCategoriesDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductWithCategoriesDto>>>> GetProductsWithCategories()
        {
            try
            {
                var products = await _productService.GetProductsWithCategoriesAsync();

                _logger.LogInformation("Retrieved {Count} products with categories", products.Count());

                return Ok(ApiResponse<IEnumerable<ProductWithCategoriesDto>>.SuccessResult(
                    products,
                    "Products with categories retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products with categories");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving products"));
            }
        }

        /// <summary>
        /// Get a specific product by ID with its associated categories
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product with categories</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductWithCategoriesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ProductWithCategoriesDto>>> GetProductWithCategories(int id)
        {
            try
            {
                var product = await _productService.GetProductWithCategoriesByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Product not found"));
                }

                _logger.LogInformation("Retrieved product {ProductId} with categories", id);

                return Ok(ApiResponse<ProductWithCategoriesDto>.SuccessResult(
                    product,
                    "Product with categories retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product {ProductId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving the product"));
            }
        }

        /// <summary>
        /// Get products by category (Many-to-Many query)
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Products in the specified category</returns>
        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductWithCategoriesDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductWithCategoriesDto>>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);

                _logger.LogInformation("Retrieved {Count} products for category {CategoryId}", products.Count(), categoryId);

                return Ok(ApiResponse<IEnumerable<ProductWithCategoriesDto>>.SuccessResult(
                    products,
                    $"Products for category retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for category {CategoryId}", categoryId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving products"));
            }
        }
    }
}