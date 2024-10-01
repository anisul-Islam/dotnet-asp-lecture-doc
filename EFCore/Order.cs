using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.EFCore;

namespace ecommerce_db_api.Models.orders
{
    public class Order
    {
        public int OrderId { get; set; } // Primary Key
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}