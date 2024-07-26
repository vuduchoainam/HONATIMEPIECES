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
        private readonly ApplicationDbContext _context;

        public BrandService(IRepository<Brand> brandRepository, ApplicationDbContext context)
        {
            _brandRepository = brandRepository;
            _context = context;
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
            if (brand != null)
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

        public async Task<PagedResult<BrandDTO>> SearchAsync(SearchBrandDTO searchBrandDto)
        {
            if (searchBrandDto.PageSize <= 0)
            {
                searchBrandDto.PageSize = 10;
            }

            if (searchBrandDto.PageNumber < 0)
            {
                searchBrandDto.PageNumber = 0;
            }

            var query = _brandRepository.GetQueryable()
                                        .Include(b => b.Images) // join bới UploadImage để hiển thị imageURL trong Brand
                                        .AsQueryable();

            // Filter by Id
            if (searchBrandDto.Id.HasValue)
            {
                query = query.Where(b => b.Id == searchBrandDto.Id.Value);
            }

            // Search by keyword
            if (!string.IsNullOrEmpty(searchBrandDto.KeyWord))
            {
                query = query.Where(b => b.Name.Contains(searchBrandDto.KeyWord));
            }

            // Filter by categoryId
            if (searchBrandDto.CategoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId.Equals(searchBrandDto.CategoryId));
            }

            // Sort (ASC/DESC)
            bool desc = searchBrandDto.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchBrandDto.OrderBy, desc);

            // Count total resources
            var totalCount = await query.CountAsync();
            var skip = searchBrandDto.PageNumber * searchBrandDto.PageSize;
            var items = await query.Skip(skip).Take(searchBrandDto.PageSize).ToListAsync();

            // Map entities to DTOs
            var resultItems = items.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name,
                Slug = b.Slug,
                Description = b.Description,
                CategoryId = b.CategoryId,
                CreatedAt = b.CreatedAt ?? DateTime.Now,
                UpdatedAt = b.UpdatedAt ?? DateTime.Now,
                ImageUrls = b.Images.Select(img => img.ImageUrl).ToList()
            }).ToList();

            var result = new PagedResult<BrandDTO>
            {
                PageNumber = searchBrandDto.PageNumber,
                PageSize = searchBrandDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchBrandDto.PageSize),
                Items = resultItems
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
