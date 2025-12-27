using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost()]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddCategory([FromForm]CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation Error", errors));
            }
            var result = await _categoryService.AddCategoryAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(201, result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetCategory()
        {
          
            var result = await _categoryService.GetCategoryAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {

            var result = await _categoryService.GetById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateCategory(int id,[FromForm] CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation Error", errors));
            }
            var result = await _categoryService.Update(id,dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var result = await _categoryService.Delete(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
    }
}
