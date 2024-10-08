using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.Models.orders;

namespace ecommerce_db_api.EFCore
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public bool IsBanned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Order> orders { get; set; } = new List<Order>();
    }
}