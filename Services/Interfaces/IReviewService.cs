namespace eShop.Services.Interfaces
{
    public interface IReviewService
    {
        public Task<GeneralResponse<object>> AddReview(AddReviewDto dto);
        public Task<GeneralResponse<object>> GetProductReviews(int productId);
    }
}
