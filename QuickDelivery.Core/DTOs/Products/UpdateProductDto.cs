using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.DTOs.Products
{
    public class UpdateProductDto
    {
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal? Price { get; set; }

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        public bool? IsAvailable { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number")]
        public int? StockQuantity { get; set; }

        public List<int>? CategoryIds { get; set; } // Pentru many-to-many relationship cu categoriile
    }
}