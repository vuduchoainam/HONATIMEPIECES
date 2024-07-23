
using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Services;
using Microsoft.AspNetCore.Mvc;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpPost("SearchBrand")]
        public async Task<IActionResult> Search([FromBody] SearchBrandDTO searchBrandDto)
        {
            try
            {
                var pagedResult = await _brandService.SearchAsync(searchBrandDto);
                var items = pagedResult.Items.Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Slug,
                    b.CategoryId,
                    b.Description,
                    CreatedAt = StringUtil.FormatDate(b.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(b.UpdatedAt),
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
        public async Task<IActionResult> Create([FromBody] CreateBrandDTO createBrandDto)
        {
            if(string.IsNullOrEmpty(createBrandDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var brand = new Brand
                {
                    Name = createBrandDto.Name,
                    Description = createBrandDto?.Description,
                    Slug = StringUtil.GenerateSlug(createBrandDto.Name),
                    CategoryId = createBrandDto.CategoryId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var originalSlug = brand.Slug;
                var slugIndex = 1;
                while (await _brandService.CountWithSlugAsync(brand.Slug) > 0)
                {
                    // Nếu slug đã tồn tại
                    brand.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                await _brandService.AddAsync(brand);
                await _brandService.SaveChangesAsync();

                var result = new
                {
                    brand.Id,
                    brand.Name,
                    brand.Slug,
                    brand.Description,
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
        public async Task<IActionResult> Edit(int id,[FromBody] CreateBrandDTO eBrand)
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
                    return StatusCodeResponse.NotFoundResponse("Brand not foung", "Brand not found");
                }
                brand.Name = eBrand.Name;
                brand.Slug = StringUtil.GenerateSlug(eBrand.Name);
                brand.Description = eBrand.Description;
                brand.UpdatedAt = DateTime.Now;

                await _brandService.UpdateAsync(brand);
                await _brandService.SaveChangesAsync();

                var result = new
                {
                    brand.Id,
                    brand.Name,
                    brand.Slug,
                    brand.Description,
                    CreateAt = StringUtil.FormatDate(brand.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(brand.UpdatedAt)
                };
                return StatusCodeResponse.SuccessResponse(result, "Edit Brand successfully");
            }catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpDelete("DeleteBrand/{id}")]
        public async Task<IActionResult> Delete (int id)
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
            }catch(Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

    }
}
