﻿using System.ComponentModel.DataAnnotations;

namespace HONATIMEPIECES.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}