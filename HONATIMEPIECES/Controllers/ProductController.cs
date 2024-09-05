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
using HONATIMEPIECES.DTOs.BrandDTO;
using Microsoft.EntityFrameworkCore;
using HONATIMEPIECES.Data;
using HONATIMEPIECES.Services;
using HONATIMEPIECES.DTOs.PropertyProductDTO;
using Models = HONATIMEPIECES.Models;
using Microsoft.AspNetCore.Authorization;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRepository<UploadImage> _uploadImageRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly IPropertyProductService _propertyProductService;

        public ProductController(IProductService productService, IRepository<UploadImage> productImageRepository, IWebHostEnvironment environment, ApplicationDbContext context, IPropertyProductService propertyProductService)
        {
            _productService = productService;
            _uploadImageRepository = productImageRepository;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
            _propertyProductService = propertyProductService;
        }

        [HttpPost("SearchProduct")]
        public async Task<IActionResult> Search([FromBody] DTOs.ProductDTO.PropertyProduct searchProductDto)
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
                    BrandName = p.Brand != null ? p.Brand.Name : null,
                    p.Quantity,
                    Images = p.Images.Select(img => img.ImageUrl).ToList(),
                    p.Status,
                    PropertyProducts = p.PropertyProducts.Select(pp => new
                    {
                        pp.PropertyId,
                        PropertyName = _context.Properties.FirstOrDefault(p => p.Id == pp.PropertyId)?.Name,
                        pp.PropertyValueId,
                        PropertyValueName = _context.PropertyValues.FirstOrDefault(pv => pv.Id == pp.PropertyValueId)?.Name,
                        CombinedName = $"{_context.Properties.FirstOrDefault(p => p.Id == pp.PropertyId)?.Name} : {_context.PropertyValues.FirstOrDefault(pv => pv.Id == pp.PropertyValueId)?.Name}"
                    }).ToList(),
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
                var product = new Models.Product
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
                    Images = new List<Models.UploadImage>(),
                    PropertyProducts = new List<Models.PropertyProduct>()
                };

                var originalSlug = product.Slug;
                var slugIndex = 1;
                while (await _productService.CountWithSlugAsync(product.Slug) > 0)
                {
                    product.Slug = $"{originalSlug}-{slugIndex}";
                    slugIndex++;
                }

                var imagesFolderPath = Path.Combine(_environment.WebRootPath, "images", "product");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                foreach (var image in createProductDto.Images)
                {
                    if (image?.FileName != null)
                    {
                        // Lấy tên tệp không có phần mở rộng và phần mở rộng của tệp
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(image.FileName);
                        var fileExtension = Path.GetExtension(image.FileName);

                        // Thay thế khoảng trắng bằng dấu gạch dưới trong tên tệp
                        var sanitizedFileName = fileNameWithoutExtension.Replace(" ", "_");

                        // Tạo tên tệp tùy chỉnh với định dạng thời gian và tên tệp đã được thay thế
                        var customFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{sanitizedFileName}{fileExtension}";

                        // Tạo đường dẫn đầy đủ cho tệp hình ảnh
                        var imagePath = Path.Combine(imagesFolderPath, customFileName);

                        // Lưu tệp vào thư mục wwwroot/images/product
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Thêm URL hình ảnh vào danh sách hình ảnh của sản phẩm
                        product.Images.Add(new Models.UploadImage { ImageUrl = "/images/product/" + customFileName });
                    }
                }

                // Save product to get the ProductId
                await _productService.AddAsync(product);
                await _productService.SaveChangesAsync();

                // Add PropertyProduct entries with the obtained ProductId
                foreach (var pp in createProductDto.PropertyProducts)
                {
                    var propertyProduct = new Models.PropertyProduct
                    {
                        ProductId = product.Id,
                        PropertyId = pp.PropertyId,
                        PropertyValueId = pp.PropertyValueId
                    };

                    product.PropertyProducts.Add(propertyProduct);
                }

                // Save PropertyProduct entries
                await _propertyProductService.SaveChangesAsync();

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
                    PropertyProducts = product.PropertyProducts.Select(pp => new
                    {
                        pp.PropertyId,
                        PropertyName = _context.Properties.FirstOrDefault(p => p.Id == pp.PropertyId)?.Name,
                        pp.PropertyValueId,
                        PropertyValueName = _context.PropertyValues.FirstOrDefault(pv => pv.Id == pp.PropertyValueId)?.Name,
                        CombinedName = $"{_context.Properties.FirstOrDefault(p => p.Id == pp.PropertyId)?.Name} : {_context.PropertyValues.FirstOrDefault(pv => pv.Id == pp.PropertyValueId)?.Name}"
                    }).ToList(),
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
