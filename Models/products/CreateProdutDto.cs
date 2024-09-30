using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models.products
{
    public class CreateProdutDto
    {

        [Required]
        public string Name { get; set; }

        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int Sold { get; set; } = 0;

        public decimal Shipping { get; set; } = 0;

        [Required]
        public Guid CategoryId { get; set; }  // Foreign Key for the Category

    }
}