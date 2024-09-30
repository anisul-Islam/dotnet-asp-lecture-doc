using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ecommerce_db_api.EFCore
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1-Many relationship => 1 category has multiple products
        // One-to-Many: A category has many products
        [JsonIgnore]
        public List<Product> Products { get; set; }
    }
}