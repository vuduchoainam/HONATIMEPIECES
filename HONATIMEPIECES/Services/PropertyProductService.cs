using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.PropertyProductDTO.SearchPropertyProductDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HONATIMEPIECES.Services
{
    public class PropertyProductService : IPropertyProductService
    {
        private readonly IRepository<PropertyProduct> _propertyProductRepository;
        public PropertyProductService(IRepository<PropertyProduct> propertyProductRepository)
        {
            _propertyProductRepository = propertyProductRepository;
        }

        public async Task AddAsync(PropertyProduct propertyProduct)
        {
            await _propertyProductRepository.AddAsync(propertyProduct);
        }

        public async Task SaveChangesAsync()
        {
            await _propertyProductRepository.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _propertyProductRepository.CountAsync(pp => true); // Đếm tất cả các bản ghi
        }

        public async Task<PagedResult<PropertyProduct>> SearchAsync(SearchPropertyProductDTO searchPropertyProductDto)
        {
            if (searchPropertyProductDto.PageSize <= 0)
            {
                searchPropertyProductDto.PageSize = 10;
            }

            if (searchPropertyProductDto.PageNumber < 0)
            {
                searchPropertyProductDto.PageNumber = 0;
            }
            var query = _propertyProductRepository.GetQueryable();
            //getById
            if (searchPropertyProductDto.Id.HasValue)
            {
                query = query.Where(c => c.Id == searchPropertyProductDto.Id.Value);
            }
            //sort (ASC/DESC)
            bool desc = searchPropertyProductDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchPropertyProductDto.OrderBy, desc);
            //count total resource
            var totalCount = await query.CountAsync();
            var skip = searchPropertyProductDto.PageNumber * searchPropertyProductDto.PageSize;
            var items = await query.Skip(skip).Take(searchPropertyProductDto.PageSize).ToListAsync();

            var result = new PagedResult<PropertyProduct>
            {
                PageNumber = searchPropertyProductDto.PageNumber,
                PageSize = searchPropertyProductDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchPropertyProductDto.PageSize),
                Items = items
            };
            return result;
        }

        public async Task UpdateAsync(PropertyProduct propertyProduct)
        {
            _propertyProductRepository.UpdateAsync(propertyProduct);
            await _propertyProductRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var propertyProduct = await _propertyProductRepository.GetByIdAsync(id);
            if(propertyProduct != null)
            {
                await _propertyProductRepository.RemoveAsync(propertyProduct);
            }
        }

        public async Task<PropertyProduct> GetByIdCategoryAsync(int id)
        {
            return await _propertyProductRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PropertyProduct>> FindAsync(Expression<Func<PropertyProduct, bool>> predicate)
        {
            return await _propertyProductRepository.FindAsync(predicate);
        }
    }
}
