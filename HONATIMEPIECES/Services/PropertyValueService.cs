using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.PropertyDTO;
using HONATIMEPIECES.DTOs.PropertyValueDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using Microsoft.EntityFrameworkCore;

namespace HONATIMEPIECES.Services
{
    public class PropertyValueService : IPropertyValueService
    {
        private readonly IRepository<PropertyValue> _propertyValueRepository;
        public PropertyValueService(IRepository<PropertyValue> propertyValueRepository)
        {
            _propertyValueRepository = propertyValueRepository;
        }

        public async Task AddAsync(PropertyValue propertyValue)
        {
            await _propertyValueRepository.AddAsync(propertyValue);
        }

        public async Task DeleteAsync(int id)
        {
            var propertyValue = await _propertyValueRepository.GetByIdAsync(id);
            if (propertyValue != null)
            {
                await _propertyValueRepository.RemoveAsync(propertyValue);
            }
        }

        public async Task<PropertyValue> GetByIdAsync(int id)
        {
            return await _propertyValueRepository.GetByIdAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _propertyValueRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<PropertyValue>> SearchAsync(SearchPropertyValueDTO searchPropertyValueDto)
        {
            if (searchPropertyValueDto.PageSize <= 0)
            {
                searchPropertyValueDto.PageSize = 10;
            }

            if (searchPropertyValueDto.PageNumber < 0)
            {
                searchPropertyValueDto.PageNumber = 0;
            }
            var query = _propertyValueRepository.GetQueryable();
            //getById
            if (searchPropertyValueDto.Id.HasValue)
            {
                query = query.Where(b => b.Id == searchPropertyValueDto.Id.Value);
            }
            //search
            if (!string.IsNullOrEmpty(searchPropertyValueDto.KeyWord))
            {
                query = query.Where(b => b.Name.Contains(searchPropertyValueDto.KeyWord));
            }
            //sort (ASC/DESC)
            bool desc = searchPropertyValueDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchPropertyValueDto.OrderBy, desc);
            //count total resource
            var totalCount = await query.CountAsync();
            var skip = searchPropertyValueDto.PageNumber * searchPropertyValueDto.PageSize;
            var items = await query.Skip(skip).Take(searchPropertyValueDto.PageSize).ToListAsync();

            var result = new PagedResult<PropertyValue>
            {
                PageNumber = searchPropertyValueDto.PageNumber,
                PageSize = searchPropertyValueDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchPropertyValueDto.PageSize),
                Items = items
            };

            return result;
        }

        public async Task UpdateAsync(PropertyValue propertyValue)
        {
            _propertyValueRepository.UpdateAsync(propertyValue);
            await _propertyValueRepository.SaveChangesAsync();
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _propertyValueRepository.GetAll().AnyAsync(p => p.Name == name);
        }
    }
}
