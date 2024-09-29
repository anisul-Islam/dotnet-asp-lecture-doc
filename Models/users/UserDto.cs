using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } 
        public string? Address { get; set; }
        public string? Image { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}