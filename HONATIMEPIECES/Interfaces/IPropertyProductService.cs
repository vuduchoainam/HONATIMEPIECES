using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.PropertyProductDTO.SearchPropertyProductDTO;
using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.Interfaces
{
    public interface IPropertyProductService
    {
        Task<PagedResult<PropertyProduct>> SearchAsync(SearchPropertyProductDTO searchPropertyProductDto);
        Task AddAsync(PropertyProduct propertyProduct);
        Task UpdateAsync(PropertyProduct propertyProduct);
        Task<PropertyProduct> GetByIdCategoryAsync(int id);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<int> CountAsync();
    }
}
