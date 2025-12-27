using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddReview(AddReviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));



            }
            var result = await _reviewService.AddReview(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            
            var result = await _reviewService.GetProductReviews(productId);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
    }
}
