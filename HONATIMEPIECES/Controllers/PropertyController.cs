using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.DTOs.PropertyDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Services;
using Microsoft.AspNetCore.Mvc;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PropertyController
    {
        private readonly IPropertyService _propertyService;
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpPost("SearchProperty")]
        public async Task<IActionResult> SearchProperty ([FromBody] SearchPropertyDTO searchPropertyDTO)
        {
            try
            {
                var pagedResult = await _propertyService.SearchAsync(searchPropertyDTO);
                var items = pagedResult.Items.Select(p => new
                {
                    p.Id,
                    p.Name,
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

        [HttpPost("CreateProperty")]
        public async Task<IActionResult> Create([FromBody] CreatePropertyDTO createPropertyDto)
        {
            if (string.IsNullOrEmpty(createPropertyDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }

            try
            {
                bool nameExists = await _propertyService.NameExistsAsync(createPropertyDto.Name);
                if (nameExists)
                {
                    return StatusCodeResponse.BadRequestResponse("Tên vừa nhập đã tồn tại", "Tên vừa nhập đã tồn tại");
                }

                var property = new Property
                {
                    Name = createPropertyDto.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _propertyService.AddAsync(property);
                await _propertyService.SaveChangesAsync();

                var result = new
                {
                    property.Id,
                    property.Name,
                    CreatedAt = StringUtil.FormatDate(property.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(property.UpdatedAt)
                };
                return StatusCodeResponse.CreatedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpPut("EditProperty/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CreatePropertyDTO eProperty)
        {
            if (string.IsNullOrEmpty(eProperty.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var property = await _propertyService.GetByIdAsync(id);
                if (property == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Property not found", "Property not found");
                }
                property.Name = eProperty.Name;
                property.UpdatedAt = DateTime.Now;

                await _propertyService.UpdateAsync(property);
                await _propertyService.SaveChangesAsync();

                var result = new
                {
                    property.Id,
                    property.Name,
                    CreateAt = StringUtil.FormatDate(property.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(property.UpdatedAt)
                };
                return StatusCodeResponse.SuccessResponse(result, "Edit Property successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpDelete("DeleteProperty/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var property = await _propertyService.GetByIdAsync(id);
                if (property == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Property not found", "Property not found");
                }

                await _propertyService.DeleteAsync(id);
                await _propertyService.SaveChangesAsync();

                return StatusCodeResponse.NoContentResponse("Delete Property successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }
    }
}
