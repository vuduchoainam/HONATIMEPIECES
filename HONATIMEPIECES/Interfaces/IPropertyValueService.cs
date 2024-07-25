using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.PropertyValueDTO;
using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.Interfaces
{
    public interface IPropertyValueService
    {
        Task<PagedResult<PropertyValue>> SearchAsync(SearchPropertyValueDTO searchPropertyValueDto); // Search + Sort + Filter
        Task AddAsync(PropertyValue propertyValue);
        Task SaveChangesAsync();
        Task<PropertyValue> GetByIdAsync(int id);
        Task UpdateAsync(PropertyValue propertyValue);
        Task DeleteAsync(int id);
        Task<bool> NameExistsAsync(string name);
    }
}
