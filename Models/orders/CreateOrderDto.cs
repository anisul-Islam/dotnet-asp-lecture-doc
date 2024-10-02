using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models.orders
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public List<CreateOrderProductDto> OrderProducts { get; set; } = new List<CreateOrderProductDto>();
    }
}