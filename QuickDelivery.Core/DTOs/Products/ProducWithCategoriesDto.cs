using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDelivery.Core.DTOs
{
    public class ProductWithCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Many-to-Many relationship data
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}

