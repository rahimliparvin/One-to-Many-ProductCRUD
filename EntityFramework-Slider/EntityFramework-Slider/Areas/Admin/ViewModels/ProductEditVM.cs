﻿using EntityFramework_Slider.Models;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProductEditVM
    { 
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public int Count { get; set; }
        [Required(ErrorMessage ="Don't be empty")]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        public List<IFormFile> Photos { get; set; }

    }
}
