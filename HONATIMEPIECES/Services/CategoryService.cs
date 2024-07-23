using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.CategoryDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using Microsoft.EntityFrameworkCore;

namespace HONATIMEPIECES.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<Category> GetByIdCategoryAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<int> CountCategoriesWithSlugAsync(string slug)
        {
            var categories = await _categoryRepository.FindAsync(x => x.Slug == slug);
            return categories.Count();
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.RemoveAsync(category);
            }
        }

        public async Task<PagedResult<Category>> SearchCategoriesAsync(SearchCategoryDTO searchCategoryDto)
        {
            if (searchCategoryDto.PageSize <= 0)
            {
                searchCategoryDto.PageSize = 10;
            }

            if (searchCategoryDto.PageNumber < 0)
            {
                searchCategoryDto.PageNumber = 0;
            }
            var query = _categoryRepository.GetQueryable();
            //getById
            if (searchCategoryDto.Id.HasValue)
            {
                query = query.Where(c => c.Id == searchCategoryDto.Id.Value);
            }
            //search
            if (!string.IsNullOrEmpty(searchCategoryDto.KeyWord))
            {
                query = query.Where(c => c.Name.Contains(searchCategoryDto.KeyWord));
            }
            //sort (ASC/DESC)
            bool desc = searchCategoryDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchCategoryDto.OrderBy, desc);
            //count total resource
            var totalCount = await query.CountAsync();
            var skip = searchCategoryDto.PageNumber * searchCategoryDto.PageSize;
            var items = await query.Skip(skip).Take(searchCategoryDto.PageSize).ToListAsync();

            var result = new PagedResult<Category>
            {
                PageNumber = searchCategoryDto.PageNumber,
                PageSize = searchCategoryDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchCategoryDto.PageSize),
                Items = items
            };

            return result;
        }

    }
}
