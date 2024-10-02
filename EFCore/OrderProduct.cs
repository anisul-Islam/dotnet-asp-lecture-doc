using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.Models.orders;

namespace ecommerce_db_api.EFCore
{
    public class OrderProduct
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Price    { get; set; }
        public int Quantity    { get; set; }
    }
}