using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.CategoryDTO;
using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<Category>> SearchCategoriesAsync(SearchCategoryDTO searchCategoryDto); // Search + Create + Sort + Filter
        Task AddCategoryAsync(Category category);
        Task SaveChangesAsync();
        Task<Category> GetByIdCategoryAsync(int id);
        Task<int> CountCategoriesWithSlugAsync(string slug);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }
}
