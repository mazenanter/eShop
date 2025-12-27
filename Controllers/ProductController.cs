using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddProduct([FromForm]ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation Error", errors));
            }
            var result = await _productService.AddAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(201, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
           [FromQuery] string? search ,
          [FromQuery] int? categoryId ,
       [FromQuery] double? minPrice ,
        [FromQuery] double? maxPrice ,
     [FromQuery] string? sortBy,
          [FromQuery] int pageNumber=1, [FromQuery] int pageSize=10)
        {
            var result = await _productService.GetAllAsync(
                search,
                categoryId,
                minPrice,
                maxPrice,sortBy,
                pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int Id )
        {
            var result = await _productService.GetById(Id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult>Delete(int id)
        {
            var result = await _productService.Delete(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id,[FromForm]ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation Error", errors));
            }
            var result = await _productService.Update(id,dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpDelete("gallery-image/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImageById(int id)
        {
            var result = await _productService.DeleteGalleryImage(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
    }
}
