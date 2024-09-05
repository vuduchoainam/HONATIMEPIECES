using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.ProductDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using HONATIMEPIECES.Repository;

namespace HONATIMEPIECES.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<UploadImage> _uploadImageRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public ProductService(IRepository<Product> productRepository, IRepository<UploadImage> productImageRepository, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _uploadImageRepository = productImageRepository;
            _environment = environment;
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task<int> CountWithSlugAsync(string slug)
        {
            var products = await _productRepository.FindAsync(x => x.Slug == slug);
            return products.Count();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            _productRepository.RemoveAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _productRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<Product>> SearchAsync(DTOs.ProductDTO.PropertyProduct searchProductDTO)
        {
            var query = _productRepository.GetQueryable()
                                        .Include(p => p.Brand)
                                        .Include(p => p.Images) // join với UploadImage để hiển thị imageURL trong Brand
                                        .Include(p => p.PropertyProducts) //join với bảng PropertyProduct
                                        .AsQueryable();
            if (searchProductDTO.PageSize <= 0)
            {
                searchProductDTO.PageSize = 10;
            }

            if (searchProductDTO.PageNumber < 0)
            {
                searchProductDTO.PageNumber = 0;
            }

            //var query = _productRepository.GetQueryable();

            // Tìm kiếm theo Id
            if (searchProductDTO.Id.HasValue)
            {
                query = query.Where(b => b.Id == searchProductDTO.Id.Value);
            }

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchProductDTO.KeyWord))
            {
                query = query.Where(b => b.Name.Contains(searchProductDTO.KeyWord));
            }

            // Tìm kiếm theo BrandId
            if (searchProductDTO.BrandId.HasValue)
            {
                query = query.Where(b => b.BrandId == searchProductDTO.BrandId.Value);
            }

            // Sắp xếp (ASC/DESC)
            bool desc = searchProductDTO.OrderByDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = query.OrderByDynamic(searchProductDTO.OrderBy, desc);

            // Đếm tổng số bản ghi
            var totalCount = await query.CountAsync();
            var skip = searchProductDTO.PageNumber * searchProductDTO.PageSize;
            var items = await query.Skip(skip).Take(searchProductDTO.PageSize).Include(p => p.Images).ToListAsync();

            // Kết quả phân trang
            var result = new PagedResult<Product>
            {
                PageNumber = searchProductDTO.PageNumber,
                PageSize = searchProductDTO.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchProductDTO.PageSize),
                Items = items
            };

            return result;
        }

        public async Task UpdateAsync(Product product)
        {
            _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }
    }
}
