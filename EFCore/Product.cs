using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ecommerce_db_api.EFCore
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public int Sold { get; set; } = 0;
        public decimal Shipping { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CategoryId { get; set; }  // Foreign Key for the Category

        [JsonIgnore]
        public Category? Category { get; set; }

    }
}