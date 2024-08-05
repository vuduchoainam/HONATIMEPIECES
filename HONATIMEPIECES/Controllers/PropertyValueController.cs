using HONATIMEPIECES.DTOs.PropertyDTO;
using HONATIMEPIECES.DTOs.PropertyValueDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PropertyValueController : Controller
    {
        private readonly IPropertyValueService _propertyValueService;
        public PropertyValueController(IPropertyValueService propertyValueService)
        {
            _propertyValueService = propertyValueService;
        }

        [HttpPost("SearchPropertyValue")]
        public async Task<IActionResult> SearchPropertyValue([FromBody] SearchPropertyValueDTO searchPropertyValueDTO)
        {
            try
            {
                var pagedResult = await _propertyValueService.SearchAsync(searchPropertyValueDTO);
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

        [HttpPost("CreatePropertyValue")]
        public async Task<IActionResult> Create([FromBody] CreatePropertyValueDTO createPropertyValueDto)
        {
            if (string.IsNullOrEmpty(createPropertyValueDto.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }

            try
            {
                bool nameExists = await _propertyValueService.NameExistsAsync(createPropertyValueDto.Name);
                if (nameExists)
                {
                    return StatusCodeResponse.BadRequestResponse("Tên vừa nhập đã tồn tại", "Tên vừa nhập đã tồn tại");
                }

                var propertyValue = new PropertyValue
                {
                    Name = createPropertyValueDto.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _propertyValueService.AddAsync(propertyValue);
                await _propertyValueService.SaveChangesAsync();

                var result = new
                {
                    propertyValue.Id,
                    propertyValue.Name,
                    CreatedAt = StringUtil.FormatDate(propertyValue.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(propertyValue.UpdatedAt)
                };
                return StatusCodeResponse.CreatedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpPut("EditPropertyValue/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CreatePropertyValueDTO ePropertyValue)
        {
            if (string.IsNullOrEmpty(ePropertyValue.Name))
            {
                return StatusCodeResponse.BadRequestResponse("Name cannot be empty", "Name cannot be empty");
            }
            try
            {
                var propertyValue = await _propertyValueService.GetByIdAsync(id);
                if (propertyValue == null)
                {
                    return StatusCodeResponse.NotFoundResponse("PropertyValue not found", "PropertyValue not found");
                }
                propertyValue.Name = ePropertyValue.Name;
                propertyValue.UpdatedAt = DateTime.Now;

                await _propertyValueService.UpdateAsync(propertyValue);
                await _propertyValueService.SaveChangesAsync();

                var result = new
                {
                    propertyValue.Id,
                    propertyValue.Name,
                    CreateAt = StringUtil.FormatDate(propertyValue.CreatedAt),
                    UpdatedAt = StringUtil.FormatDate(propertyValue.UpdatedAt)
                };
                return StatusCodeResponse.SuccessResponse(result, "Edit PropertyValue successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }

        [HttpDelete("DeletePropertyValue/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var propertyValue = await _propertyValueService.GetByIdAsync(id);
                if (propertyValue == null)
                {
                    return StatusCodeResponse.NotFoundResponse("Property not found", "Property not found");
                }

                await _propertyValueService.DeleteAsync(id);
                await _propertyValueService.SaveChangesAsync();

                return StatusCodeResponse.NoContentResponse("Delete PropertyValue successfully");
            }
            catch (Exception ex)
            {
                return StatusCodeResponse.InternalServerErrorResponse("An error occurred while processing the request", ex.Message);
            }
        }
    }
}
