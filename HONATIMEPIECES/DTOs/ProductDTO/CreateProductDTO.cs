using HONATIMEPIECES.DTOs.PropertyProductDTO;

namespace HONATIMEPIECES.DTOs.ProductDTO
{
    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<CreatePropertyProductDTO> PropertyProducts { get; set; } = new List<CreatePropertyProductDTO>();
    }

}
