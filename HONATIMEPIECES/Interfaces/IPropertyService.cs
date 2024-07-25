using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.DTOs.PropertyDTO;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;

namespace HONATIMEPIECES.Interfaces
{
    public interface IPropertyService
    {
        Task<PagedResult<Property>> SearchAsync(SearchPropertyDTO searchPropertyDto); // Search + Sort + Filter
        Task AddAsync(Property property);
        Task SaveChangesAsync();
        Task<Property> GetByIdAsync(int id);
        Task UpdateAsync(Property property);
        Task DeleteAsync(int id);
        Task<bool> NameExistsAsync(string name);
    }
}
