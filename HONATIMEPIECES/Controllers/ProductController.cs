using HONATIMEPIECES.DTOs.ProductDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using HONATIMEPIECES.Repository;

namespace HONATIMEPIECES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRepository<ProductImage> _productImageRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductService productService, IRepository<ProductImage> productImageRepository, IWebHostEnvironment environment)
        {
            _productService = productService;
            _productImageRepository = productImageRepository;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpPost("SearchProduct")]
        public async Task<IActionResult> Search([FromBody] SearchProductDTO searchProductDto)
        {
            try
            {
                var pagedResult = await _productService.SearchAsync(searchProductDto);
                var items = pagedResult.Items.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Slug,
                    p.Description,
                    p.Price,
                    p.BrandId,
                    p.Quantity,
                    Images = p.Images.Select(img => img.ImageUrl).ToList(),
                    p.Status,
                    CreatedAt = StringUtil.FormatDate(p.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(p.UpdatedAt),
                }).ToList();

                var result = new
                {
                    pagedResult.PageNumber,
                    pagedResult.PageSize,
                    pagedResult.TotalCount,
                    pagedResult.TotalPages,
                    Items = items
                };
                return StatusCodeResponse.SuccessResponse(result, "The request has been fulfilled, resulting in the creation of a new resource");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromForm] CreateProductDTO createProductDto)
        {
            if (string.IsNullOrEmpty(createProductDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var product = new Product
                {
                    Name = createProductDto.Name,
                    Slug = StringUtil.GenerateSlug(createProductDto.Name),
                    Description = createProductDto.Description,
                    Price = createProductDto.Price,
                    BrandId = createProductDto.BrandId,
                    Quantity = createProductDto.Quantity,
                    Status = createProductDto.Status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Images = new List<ProductImage>()
                };

                var originalSlug = product.Slug;
                var slugIndex = 1;
                while (await _productService.CountWithSlugAsync(product.Slug) > 0)
                {
                    product.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                var imagesFolderPath = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                foreach (var image in createProductDto.Images)
                {
                    if (image?.FileName != null)
                    {
                        // Tạo tên tệp tùy chỉnh
                        var customFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileNameWithoutExtension(image.FileName)}{Path.GetExtension(image.FileName)}";
                        var imagePath = Path.Combine(imagesFolderPath, customFileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        product.Images.Add(new ProductImage { ImageUrl = "/images/" + customFileName });
                    }
                }

                await _productService.AddAsync(product);
                await _productService.SaveChangesAsync();

                var result = new
                {
                    product.Id,
                    product.Name,
                    product.Slug,
                    product.Description,
                    product.Price,
                    product.BrandId,
                    product.Quantity,
                    Images = product.Images.Select(img => img.ImageUrl).ToList(),
                    product.Status,
                    CreatedAt = StringUtil.FormatDate(product.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(product.UpdatedAt)
                };
                return StatusCodeResponse.CreatedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

    }
}
