using System.Collections.Generic;

namespace HONATIMEPIECES.DTOs.BrandDTO
{
    public class BrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
