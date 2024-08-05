using HONATIMEPIECES.DTOs.BrandDTO;
using HONATIMEPIECES.DTOs.ProductDTO;
using HONATIMEPIECES.DTOs.PropertyProductDTO;
using HONATIMEPIECES.DTOs.PropertyProductDTO.SearchPropertyProductDTO;
using HONATIMEPIECES.Helpers;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PropertyProductController : Controller
    {
        private readonly IPropertyProductService _propertyProductService;
        public PropertyProductController(IPropertyProductService propertyProductService)
        {
            _propertyProductService = propertyProductService;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] SearchPropertyProductDTO searchPropertyProductDto)
        {
            try
            {
                var pagedResult = await _propertyProductService.SearchAsync(searchPropertyProductDto);
                var items = pagedResult.Items.Select(p => new
                {
                    p.Id,
                    p.ProductId,
                    p.PropertyId,
                    p.PropertyValueId,
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

    }


}

