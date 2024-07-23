namespace HONATIMEPIECES.DTOs.BrandDTO
{
    public class SearchBrandDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public string? KeyWord { get; set; }
        public int? CategoryId { get; set; }
        public string OrderBy { get; set; } = "Name";
        public string OrderByDirection { get; set; } = "ASC"; // Hướng sắp xếp (ASC/DESC)
    }
}
