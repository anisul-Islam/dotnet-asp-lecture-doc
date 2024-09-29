using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models
{
    public class UpdateUserDto
    {

        [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters.", MinimumLength = 3)]
        public string? UserName { get; set; } 

        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters.", MinimumLength = 6)]
        public string? Password { get; set; } 

        [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
        public string? Address { get; set; }

        public string? Image { get; set; }
    }
}