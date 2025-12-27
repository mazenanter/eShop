using AutoMapper;
using eShop.Data;
using eShop.Helpers;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public CategoryService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<object>> AddCategoryAsync(CategoryDto dto)
        {
            
            var isExist = await _context.Categories
        .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
            
            if (isExist)
            {
                return GeneralResponse<object>.Failure("Category Name is already Exists");
            }
            try
            {
                string? image = null;
                Category cat = _mapper.Map<Category>(dto);
                if (dto.Image != null)
                {
                    image = await ImageHelper.UploadImage(dto.Image, "categories");
                }
              
                cat.ImageUrl = image;
                await _context.AddAsync(cat);
                await _context.SaveChangesAsync();
                return GeneralResponse<object>.Success(cat.Id, "Category Added successfully");
            }
            catch (Exception ex)
            {

                return GeneralResponse<object>.Failure(ex.Message);
            }
        }

        

        public async Task<GeneralResponse<object>> GetCategoryAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            if (!categories.Any())
                return GeneralResponse<object>.Success(new List<object>(), "No categories found.");
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var result = categories.Select(c => new
            {
                c.Id,
                c.Name,
                
                ImageUrl = string.IsNullOrEmpty(c.ImageUrl) ? null : $"{baseUrl}{c.ImageUrl}"
            }).ToList();
            return GeneralResponse<object>.Success(result, "Categories retrieved successfully");
        }
        public async Task<GeneralResponse<object>> GetById(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (category is null)
                return GeneralResponse<object>.Success(new List<object>(), $"No category found with id:{id}");
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var result = new
            {
                category.Id,
                category.Name,

                ImageUrl = string.IsNullOrEmpty(category.ImageUrl) ? null : $"{baseUrl}{category.ImageUrl}"
            };
            return GeneralResponse<object>.Success(result, "Category retrieved successfully");
        }

        public async Task<GeneralResponse<object>> Update(int id, CategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
                return GeneralResponse<object>.Success(new List<object>(), $"No category found with id:{id}");
            if (category.Name.ToLower() != dto.Name.ToLower())
            {
                var isExist = await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
                if (isExist) return GeneralResponse<object>.Failure("Category name already exists");
                category.Name = dto.Name;
            }
            if (dto.Image != null)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                    ImageHelper.DeleteImage(category.ImageUrl);
                category.ImageUrl = await ImageHelper.UploadImage(dto.Image, "categories");
            }

            _context.Update(category);
          await  _context.SaveChangesAsync();
            return GeneralResponse<object>.Success(null, "Category Updated Successfully");
        }

        public async Task<GeneralResponse<object>> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
                return GeneralResponse<object>.Success(new List<object>(), $"No category found with id:{id}");
            if(category.ImageUrl is not null)
            {
                ImageHelper.DeleteImage(category.ImageUrl);
            }
            _context.Remove(category);
           await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success(category, "Category Deleted Successfully");
        }
    }
}
