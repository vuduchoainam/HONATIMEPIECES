using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.PropertyDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using Microsoft.EntityFrameworkCore;

namespace HONATIMEPIECES.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IRepository<Property> _propertyRepository;
        public PropertyService(IRepository<Property> propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task AddAsync(Property property)
        {
            await _propertyRepository.AddAsync(property);
        }

        public async Task DeleteAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if(property != null)
            {
                await _propertyRepository.RemoveAsync(property);
            }
        }

        public async Task<Property> GetByIdAsync(int id)
        {
            return await _propertyRepository.GetByIdAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _propertyRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<Property>> SearchAsync(SearchPropertyDTO searchPropertyDto)
        {
            if (searchPropertyDto.PageSize <= 0)
            {
                searchPropertyDto.PageSize = 10;
            }

            if (searchPropertyDto.PageNumber < 0)
            {
                searchPropertyDto.PageNumber = 0;
            }
            var query = _propertyRepository.GetQueryable();
            //getById
            if (searchPropertyDto.Id.HasValue)
            {
                query = query.Where(b => b.Id == searchPropertyDto.Id.Value);
            }
            //search
            if (!string.IsNullOrEmpty(searchPropertyDto.KeyWord))
            {
                query = query.Where(b => b.Name.Contains(searchPropertyDto.KeyWord));
            }
            //sort (ASC/DESC)
            bool desc = searchPropertyDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchPropertyDto.OrderBy, desc);
            //count total resource
            var totalCount = await query.CountAsync();
            var skip = searchPropertyDto.PageNumber * searchPropertyDto.PageSize;
            var items = await query.Skip(skip).Take(searchPropertyDto.PageSize).ToListAsync();

            var result = new PagedResult<Property>
            {
                PageNumber = searchPropertyDto.PageNumber,
                PageSize = searchPropertyDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchPropertyDto.PageSize),
                Items = items
            };

            return result;
        }

        public async Task UpdateAsync(Property property)
        {
            _propertyRepository.UpdateAsync(property);
            await _propertyRepository.SaveChangesAsync();
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _propertyRepository.GetAll().AnyAsync(p => p.Name == name);
        }
    }
}
