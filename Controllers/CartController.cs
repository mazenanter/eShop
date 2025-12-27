using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _cartService.AddToCartAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAll()
        {
            
            var result = await _cartService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpDelete("{productId:int}")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> DeleteItem(int productId)
        {
            var result = await _cartService.DeleteItem(productId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateQuantity(UpdateCartDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _cartService.UpdateProductQty(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return StatusCode(200, result);
        }
    }
}
