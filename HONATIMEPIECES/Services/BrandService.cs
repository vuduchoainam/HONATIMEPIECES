using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using Microsoft.EntityFrameworkCore;

namespace HONATIMEPIECES.Services
{
    public class BrandService : IBrandService
    {
        private readonly IRepository<Brand> _brandRepository;
        public BrandService(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task AddAsync(Brand brand)
        {
            await _brandRepository.AddAsync(brand);
        }

        public async Task<int> CountWithSlugAsync(string slug)
        {
            var brands = await _brandRepository.FindAsync(x => x.Slug == slug);
            return brands.Count();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if(brand != null)
            {
                await _brandRepository.RemoveAsync(brand);
            }
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _brandRepository.GetByIdAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _brandRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<Brand>> SearchAsync(SearchBrandDTO searchBrandDto)
        {
            if (searchBrandDto.PageSize <= 0)
            {
                searchBrandDto.PageSize = 10;
            }

            if (searchBrandDto.PageNumber < 0)
            {
                searchBrandDto.PageNumber = 0;
            }
            var query = _brandRepository.GetQueryable();
            //getById
            if (searchBrandDto.Id.HasValue)
            {
                query = query.Where(b => b.Id == searchBrandDto.Id.Value);
            }
            //search
            if (!string.IsNullOrEmpty(searchBrandDto.KeyWord))
            {
                query = query.Where(b => b.Name.Contains(searchBrandDto.KeyWord));
            }
            //search by categoryId
            if (searchBrandDto.CategoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId.Equals(searchBrandDto.CategoryId));
            }
            //sort (ASC/DESC)
            bool desc = searchBrandDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchBrandDto.OrderBy, desc);
            //count total resource
            var totalCount = await query.CountAsync();
            var skip = searchBrandDto.PageNumber * searchBrandDto.PageSize;
            var items = await query.Skip(skip).Take(searchBrandDto.PageSize).ToListAsync();

            var result = new PagedResult<Brand>
            {
                PageNumber = searchBrandDto.PageNumber,
                PageSize = searchBrandDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchBrandDto.PageSize),
                Items = items
            };

            return result;
        }

        public async Task UpdateAsync(Brand brand)
        {
            _brandRepository.UpdateAsync(brand);
            await _brandRepository.SaveChangesAsync();
        }
    }
}
