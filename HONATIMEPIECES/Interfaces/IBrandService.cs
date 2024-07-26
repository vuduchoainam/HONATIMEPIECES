using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.Interfaces
{
    public interface IBrandService
    {
        Task<PagedResult<BrandDTO>> SearchAsync(SearchBrandDTO searchBrandDto);
        Task AddAsync(Brand brand);
        Task SaveChangesAsync();
        Task<Brand> GetByIdAsync(int id);
        Task<int> CountWithSlugAsync(string slug);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(int id);
    }
}
