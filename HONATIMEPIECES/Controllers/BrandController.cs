using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using HONATIMEPIECES.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HONATIMEPIECES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<UploadImage> _uploadImageRepository;

        public BrandController(IBrandService brandService, IWebHostEnvironment environment, IRepository<UploadImage> brandImageRepository)
        {
            _brandService = brandService;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _uploadImageRepository = brandImageRepository;
        }

        [HttpPost("SearchBrand")]
        public async Task<IActionResult> Search([FromBody] SearchBrandDTO searchBrandDto)
        {
            try
            {
                var pagedResult = await _brandService.SearchAsync(searchBrandDto);
                var items = pagedResult.Items.Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Slug = b.Slug,
                    Description = b.Description,
                    CategoryId = b.CategoryId,
                    ImageUrls = b.ImageUrls,
                    CreatedAt = b.CreatedAt ?? DateTime.Now,
                    UpdatedAt = b.UpdatedAt ?? DateTime.Now,
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


        [HttpPost("CreateBrand")]
        public async Task<IActionResult> Create([FromForm] CreateBrandDTO createBrandDto)
        {
            if (string.IsNullOrEmpty(createBrandDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var brand = new Brand
                {
                    Name = createBrandDto.Name,
                    Description = createBrandDto.Description,
                    Slug = StringUtil.GenerateSlug(createBrandDto.Name),
                    CategoryId = createBrandDto.CategoryId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Images = new List<UploadImage>()
                };

                var originalSlug = brand.Slug;
                var slugIndex = 1;
                while (await _brandService.CountWithSlugAsync(brand.Slug) > 0)
                {
                    brand.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                var imagesFolderPath = Path.Combine(_environment.WebRootPath, "images", "brand");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                foreach (var image in createBrandDto.Images)
                {
                    if (image?.FileName != null)
                    {
                        var customFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileNameWithoutExtension(image.FileName)}{Path.GetExtension(image.FileName)}";
                        var imagePath = Path.Combine(imagesFolderPath, customFileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        brand.Images.Add(new UploadImage { ImageUrl = "/images/brand/" + customFileName });
                    }
                }

                await _brandService.AddAsync(brand);
                await _brandService.SaveChangesAsync();

                var result = new
                {
                    brand.Id,
                    brand.Name,
                    brand.Slug,
                    brand.Description,
                    Images = brand.Images.Select(img => img.ImageUrl).ToList(),
                    brand.CategoryId,
                    CreatedAt = StringUtil.FormatDate(brand.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(brand.UpdatedAt)
                };
                return StatusCodeResponse.CreatedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpPut("EditBrand/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] CreateBrandDTO eBrand)
        {
            if (string.IsNullOrEmpty(eBrand.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var brand = await _brandService.GetByIdAsync(id);
                if (brand == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Brand not found", "Brand not found");
                }

                brand.Name = eBrand.Name;
                brand.Slug = StringUtil.GenerateSlug(eBrand.Name);
                brand.Description = eBrand.Description;
                brand.UpdatedAt = DateTime.Now;

                var imagesFolderPath = Path.Combine(_environment.WebRootPath, "images", "brand");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                foreach (var image in eBrand.Images)
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

                        brand.Images.Add(new UploadImage { ImageUrl = "/images/brand/" + customFileName });
                    }
                }

                await _brandService.UpdateAsync(brand);
                await _brandService.SaveChangesAsync();

                var result = new
                {
                    brand.Id,
                    brand.Name,
                    brand.Slug,
                    brand.Description,
                    Images = brand.Images.Select(img => img.ImageUrl).ToList(),
                    CreateAt = StringUtil.FormatDate(brand.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(brand.UpdatedAt)
                };
                return StatusCodeResponse.SuccessResponse(result, "Edit Brand successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpDelete("DeleteBrand/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var brand = await _brandService.GetByIdAsync(id);
                if (brand == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Brand not found", "Brand not found");
                }

                await _brandService.DeleteAsync(id);
                await _brandService.SaveChangesAsync();

                return StatusCodeResponse.NoContentResponse("Delete Brand successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }
    }
}
