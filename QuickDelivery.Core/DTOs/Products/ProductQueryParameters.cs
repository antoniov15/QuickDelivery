using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.DTOs.Products
{
    public class ProductQueryParameters
    {
        // Pagination
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        // Filtering
        public string? SearchTerm { get; set; } // Filter by name or description
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }
        public string? CategoryName { get; set; }
        public string? PartnerName { get; set; }

        // Sorting
        public string SortBy { get; set; } = "name"; // name, price, createdAt, stockQuantity
        public string SortOrder { get; set; } = "asc"; // asc, desc

        // Computed properties for easier use
        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }
}
