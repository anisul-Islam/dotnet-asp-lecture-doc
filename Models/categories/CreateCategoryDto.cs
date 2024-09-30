using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_db_api.Models
{
    public class CreateCategoryDto
    {

        [Required(ErrorMessage = "CategoryName is missing.")]
        [StringLength(100, ErrorMessage = "CategoryName must be between 3 and 100 characters.", MinimumLength = 3)]
        public string Name { get; set; }
        public string? Slug { get; set; }

    }
}