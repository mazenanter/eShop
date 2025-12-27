namespace eShop.Services.Interfaces
{
    public interface IProductService
    {
        public Task<GeneralResponse<object>> GetAllAsync(
            string? search = null,
            int? categoryId = null,
        double? minPrice = null,
         double? maxPrice = null,
      string? sortBy = null,
            int pageNumber=1, int pageSize=10);
        public Task<GeneralResponse<object>> AddAsync(ProductDto dto);
        public Task<GeneralResponse<object>> GetById(int id);
        public Task<GeneralResponse<object>> Delete(int id);
        public Task<GeneralResponse<object>> Update(int id, ProductDto dto);
        public  Task<GeneralResponse<object>> DeleteGalleryImage(int imageId);
    }
}
