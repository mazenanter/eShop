namespace eShop.Services.Interfaces
{
    public interface ICartService
    {
        public Task<GeneralResponse<object>> AddToCartAsync(AddToCartDto dto);
        public Task<GeneralResponse<object>> GetAllAsync();
        public Task<GeneralResponse<object>> DeleteItem(int productId);
        public Task<GeneralResponse<object>> UpdateProductQty(UpdateCartDto dto);
    }
}
