
namespace eShop.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<GeneralResponse<object>> AddCategoryAsync(CategoryDto dto);
        public Task<GeneralResponse<object>> GetCategoryAsync();
        public Task<GeneralResponse<object>> GetById(int id);
        public Task<GeneralResponse<object>> Update(int id, CategoryDto dto);
        public Task<GeneralResponse<object>> Delete(int id);
    }
}
