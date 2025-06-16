using Microsoft.AspNetCore.Mvc;
using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Products;
using QuickDelivery.Core.Interfaces.Services;
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
        /// Get products with advanced filtering, pagination, and sorting
        /// Supports filtering by: search term, price range, availability, category, partner
        /// Supports sorting by: name, price, creation date, stock quantity
        /// Supports pagination with page and pageSize parameters
        /// </summary>
        /// <param name="parameters">Query parameters for filtering, sorting, and pagination</param>
        /// <returns>Paginated list of products with categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductWithCategoriesDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<ProductWithCategoriesDto>>>> GetProducts([FromQuery] ProductQueryParameters parameters)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid parameters", errors));
                }

                // Get paginated products with filtering and sorting
                var result = await _productService.GetProductsWithCategoriesAsync(parameters);

                _logger.LogInformation("Retrieved {Count} products out of {Total} with pagination (page {Page}, size {PageSize})",
                    result.Data.Count(), result.TotalCount, result.Page, result.PageSize);

                return Ok(ApiResponse<PaginatedResult<ProductWithCategoriesDto>>.SuccessResult(
                    result,
                    $"Products retrieved successfully. Page {result.Page} of {result.TotalPages}, showing {result.Data.Count()} of {result.TotalCount} total products."
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products with parameters: {@Parameters}", parameters);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving products"));
            }
        }

        /// <summary>
        /// Get all products with their associated categories (Many-to-Many relationship) - Simple version
        /// This endpoint maintains backwards compatibility with the original implementation
        /// </summary>
        /// <returns>List of products with categories</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductWithCategoriesDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductWithCategoriesDto>>>> GetAllProductsWithCategories()
        {
            try
            {
                var products = await _productService.GetProductsWithCategoriesAsync();

                _logger.LogInformation("Retrieved {Count} products with categories (simple)", products.Count());

                return Ok(ApiResponse<IEnumerable<ProductWithCategoriesDto>>.SuccessResult(
                    products,
                    "Products with categories retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products with categories (simple)");
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
        /// Update a product completely (replaces all fields)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated product with categories</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductWithCategoriesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ProductWithCategoriesDto>>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid product data", errors));
                }

                // Update the product
                var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);

                if (updatedProduct == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Product not found"));
                }

                _logger.LogInformation("Product {ProductId} updated successfully by user. Updated fields: {@UpdateData}",
                    id, updateProductDto);

                return Ok(ApiResponse<ProductWithCategoriesDto>.SuccessResult(
                    updatedProduct,
                    "Product updated successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product {ProductId} with data: {@UpdateData}",
                    id, updateProductDto);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating the product"));
            }
        }

        /// <summary>
        /// Update a product partially (updates only provided fields)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Partial product update data</param>
        /// <returns>Updated product with categories</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductWithCategoriesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ProductWithCategoriesDto>>> PartialUpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid product data", errors));
                }

                // Validate that at least one field is provided for patch
                if (updateProductDto.Name == null &&
                    updateProductDto.Description == null &&
                    updateProductDto.Price == null &&
                    updateProductDto.ImageUrl == null &&
                    updateProductDto.Category == null &&
                    updateProductDto.IsAvailable == null &&
                    updateProductDto.StockQuantity == null &&
                    (updateProductDto.CategoryIds == null || !updateProductDto.CategoryIds.Any()))
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("At least one field must be provided for partial update"));
                }

                // Update the product partially
                var updatedProduct = await _productService.PartialUpdateProductAsync(id, updateProductDto);

                if (updatedProduct == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Product not found"));
                }

                _logger.LogInformation("Product {ProductId} partially updated successfully by user. Updated fields: {@UpdateData}",
                    id, updateProductDto);

                return Ok(ApiResponse<ProductWithCategoriesDto>.SuccessResult(
                    updatedProduct,
                    "Product updated successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while partially updating product {ProductId} with data: {@UpdateData}",
                    id, updateProductDto);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating the product"));
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

        /// <summary>
        /// Example endpoint for demonstrating different filtering and update options
        /// </summary>
        /// <returns>Usage examples</returns>
        [HttpGet("examples")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult GetFilteringExamples()
        {
            var examples = new
            {
                message = "Product filtering, sorting, pagination and update examples",
                filteringExamples = new[]
                {
                    new {
                        description = "Get first page with 5 products, sorted by price descending",
                        url = "/api/products?page=1&pageSize=5&sortBy=price&sortOrder=desc"
                    },
                    new {
                        description = "Filter by price range and availability",
                        url = "/api/products?minPrice=10&maxPrice=30&isAvailable=true"
                    },
                    new {
                        description = "Search for pizza products",
                        url = "/api/products?searchTerm=pizza"
                    },
                    new {
                        description = "Filter by category and partner, sort by name",
                        url = "/api/products?categoryName=Fast%20Food&partnerName=restaurant&sortBy=name&sortOrder=asc"
                    },
                    new {
                        description = "Combined filtering with pagination",
                        url = "/api/products?searchTerm=chicken&minPrice=15&isAvailable=true&page=1&pageSize=10&sortBy=createdAt&sortOrder=desc"
                    }
                },
                updateExamples = new
                {
                    putExample = new
                    {
                        description = "Complete product update (PUT /api/products/{id})",
                        method = "PUT",
                        url = "/api/products/1",
                        body = new
                        {
                            name = "Updated Product Name",
                            description = "Updated description",
                            price = 29.99,
                            imageUrl = "https://example.com/updated-image.jpg",
                            category = "Updated Category",
                            isAvailable = true,
                            stockQuantity = 50,
                            categoryIds = new[] { 1, 2, 3 }
                        }
                    },
                    patchExample = new
                    {
                        description = "Partial product update (PATCH /api/products/{id})",
                        method = "PATCH",
                        url = "/api/products/1",
                        body = new
                        {
                            price = 39.99,
                            isAvailable = false,
                            stockQuantity = 0
                        }
                    }
                },
                availableFilters = new
                {
                    searchTerm = "Filter by product name or description",
                    minPrice = "Filter by minimum price",
                    maxPrice = "Filter by maximum price",
                    isAvailable = "Filter by availability (true/false)",
                    categoryName = "Filter by category name",
                    partnerName = "Filter by partner business name"
                },
                availableSorting = new
                {
                    sortBy = new[] { "name", "price", "createdAt", "stockQuantity" },
                    sortOrder = new[] { "asc", "desc" }
                },
                pagination = new
                {
                    page = "Page number (starting from 1)",
                    pageSize = "Number of items per page (1-100)"
                }
            };

            return Ok(examples);
        }
    }
}