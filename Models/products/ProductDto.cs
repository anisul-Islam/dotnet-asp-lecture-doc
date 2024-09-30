using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.EFCore;

namespace ecommerce_db_api.Models.products
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public decimal Shipping { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category? Category { get; set; }
        public Guid CategoryId { get; set; }  // Foreign Key for the Category

    }
}