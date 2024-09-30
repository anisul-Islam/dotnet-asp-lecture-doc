using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models.categories
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string? Name { get; set; } 
        public string? Slug { get; set; } 
        public DateTime CreatedAt { get; set; }

        // 1-Many relationship => 1 category has multiple products
        // One-to-Many: A category has many products
        // public List<Product> Products { get; set; } = new List<Product>();
    }
}