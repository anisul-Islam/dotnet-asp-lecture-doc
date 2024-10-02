using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ecommerce_db_api.EFCore;

namespace ecommerce_db_api.Models.orders
{
    public class Order
    {
        public Guid OrderId { get; set; } // Primary Key

        public DateTime OrderDate { get; set; }

        // Foreign Key
        public Guid UserId { get; set; }

        // Navigation Property
        public User User { get; set; }

        [JsonIgnore]
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}