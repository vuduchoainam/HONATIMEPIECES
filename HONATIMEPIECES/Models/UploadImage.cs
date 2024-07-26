using System.ComponentModel.DataAnnotations;

namespace HONATIMEPIECES.Models
{
    public class UploadImage
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int? BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
