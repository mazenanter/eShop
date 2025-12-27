using eShop.Data;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public ReviewService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<GeneralResponse<object>> AddReview(AddReviewDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if (userId == null) return GeneralResponse<object>.Failure("User not found please log in");
            var hasPurchased = await _context.Orders
        .AnyAsync(o => o.UserId == userId &&
                       o.OrderStatus == OrderStatus.Delivered &&
                       o.OrderItems.Any(oi => oi.ProductId == dto.ProductId));

            if (!hasPurchased)
                return GeneralResponse<object>.Failure("You can only review products you have actually purchased.");
            var exist = await _context.Reviews.AnyAsync(x => x.UserId == userId && x.ProductId == dto.ProductId);
            if (exist) return GeneralResponse<object>.Failure("You already reviewed this product");
            var review = new Review
            {
                Comment = dto.Comment,
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                
            };
           await _context.Reviews.AddAsync(review);
           await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success("Review added successfully");
        }

        public async Task<GeneralResponse<object>> GetProductReviews(int productId)
        {
            var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .Select(r => new
            {
                r.User.UserName,
                r.Rating,
                r.Comment,
                
                r.CreatedAt
            })
            .ToListAsync();

            return GeneralResponse<object>.Success(reviews);
        }
    }
}
