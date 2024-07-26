using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.DTOs.BrandDTO
{
    public class CreateBrandDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public int CategoryId { get; set; }
    }
}
