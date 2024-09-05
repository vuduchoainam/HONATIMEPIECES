using HONATIMEPIECES.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.CategoryDTO;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [HttpPost("SearchCategory")]
        public async Task<IActionResult> Search([FromBody] SearchCategoryDTO searchCategoryDto)
        {
            try
            {
                var pagedResult = await _categoryService.SearchCategoriesAsync(searchCategoryDto);

                var items = pagedResult.Items.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Slug,
                    CreatedAt = StringUtil.FormatDate(c.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(c.UpdatedAt)
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

        /*
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO createCategoryDto)
        {
            if (string.IsNullOrEmpty(createCategoryDto.Name))
            {
                var badRequestResponse = new ResponseAPI<object>
                {
                    Message = "Category name cannot be empty",
                    Result = null,
                    Error = "Category name cannot be empty",
                    StatusCode = 400
                };
                return BadRequest(badRequestResponse);
            }
            try
            {
                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Slug = GenerateSlug(createCategoryDto.Name),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var originalSlug = category.Slug;
                var slugIndex = 1;
                while (await _categoryService.CountCategoriesWithSlugAsync(category.Slug) > 0)
                {
                    // Nếu slug đã tồn tại, thử với một phiên bản mới
                    category.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                await _categoryService.AddCategoryAsync(category);
                await _categoryService.SaveChangesAsync();

                var response = new ResponseAPI<Category>
                {
                    Message = "Create Category successfully",
                    Result = category,
                    Error = null,
                    StatusCode = 200
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseAPI<object>
                {
                    Message = "An error occurred while processing the request",
                    Result = null,
                    Error = ex.Message,
                    StatusCode = 500
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        */
        
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO createCategoryDto)
        {
            if (string.IsNullOrEmpty(createCategoryDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Category name cannot be empty", "Category name cannot be empty");
            }
            try
            {
                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description,
                    Slug = StringUtil.GenerateSlug(createCategoryDto.Name),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var originalSlug = category.Slug;
                var slugIndex = 1;
                while (await _categoryService.CountCategoriesWithSlugAsync(category.Slug) > 0)
                {
                    // Nếu slug đã tồn tại
                    category.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                await _categoryService.AddCategoryAsync(category);
                await _categoryService.SaveChangesAsync();

                var result = new
                {
                    category.Id,
                    category.Name,
                    category.Slug,
                    category.Description,
                    CreatedAt = StringUtil.FormatDate(category.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(category.UpdatedAt)
                };

                return StatusCodeResponse.CreatedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpPut("EditCategory/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CreateCategoryDTO editCategoryDto)
        {
            if (string.IsNullOrEmpty(editCategoryDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Category name cannot be empty", "Category name cannot be empty");
            }
            try
            {
                var category = await _categoryService.GetByIdCategoryAsync(id);
                if (category == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Category not found", "Category not found");
                }

                category.Name = editCategoryDto.Name;
                category.Description = editCategoryDto.Description;
                category.Slug = StringUtil.GenerateSlug(editCategoryDto.Name);
                category.UpdatedAt = DateTime.Now;

                await _categoryService.UpdateCategoryAsync(category);
                await _categoryService.SaveChangesAsync();

                var result = new
                {
                    category.Id,
                    category.Name,
                    category.Slug,
                    category.Description,
                    CreatedAt = StringUtil.FormatDate(category.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(category.UpdatedAt)
                };

                return StatusCodeResponse.SuccessResponse(result, "Edit Category successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }


        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdCategoryAsync(id);
                if (category == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Category not found", "Category not found");
                }

                await _categoryService.DeleteCategoryAsync(id);
                await _categoryService.SaveChangesAsync();

                return StatusCodeResponse.NoContentResponse("Delete Category successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }


    }
}
