using AutoMapper;
using eShop.Data;
using eShop.Helpers;
using eShop.Models;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace eShop.Services.implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<object>> AddAsync(ProductDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return GeneralResponse<object>.Failure("Not valid category id");
            var products = await _context.Products.AnyAsync(x => x.Name == dto.Name);
            if(products) return GeneralResponse<object>.Failure("This Product Already Exist");
            Product product = _mapper.Map<Product>(dto);
            if(dto.Images!=null && dto.Images.Any())
            {
                foreach (var image in dto.Images)
                {
                    var url = await ImageHelper.UploadImage(image, "products");
                    product.ProductImages.Add(new ProductImage { ImageUrl = url });
                }
            }
            if (dto.MainImageUrl != null)
            {
                var MainImageUrl = await ImageHelper.UploadImage(dto.MainImageUrl, "products");
                product.MainImageUrl = MainImageUrl;
            }
           
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success(product.Id, "Product Added Successfully");
        }

        

        public async Task<GeneralResponse<object>> GetAllAsync(
            string? search = null,
            int? categoryId = null,
        double? minPrice = null,
         double? maxPrice = null,
      string? sortBy = null,

            int pageNumber = 1, int pageSize = 10)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var query =  _context.Products.Include(x => x.Category).Include(r=>r.Reviews).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()));
            }
            if (categoryId.HasValue)
            {
              
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice);
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "price_asc") query= query.OrderBy(p => p.Price);
                else if(sortBy == "price_desc") query = query.OrderByDescending(p => p.Price);
            }
                var totalCount = await query.CountAsync();
            var products = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(p => new {
                p.Id,
                p.Name,
                p.Price,
                p.Qty,
                CategoryName = p.Category.Name, 
                MainImageUrl = string.IsNullOrEmpty(p.MainImageUrl) ? null : $"{baseUrl}{p.MainImageUrl}",
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewsCount = p.Reviews.Count()

            })
        .ToListAsync();
            return GeneralResponse<object>.Success(new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products
            }, "Retrieved Successfully");
        }

        public async Task<GeneralResponse<object>> GetById(int id)
        {
            var product = await _context.Products
        .Include(p => p.Category)
        .Include(p => p.ProductImages)
        .Include(r=>r.Reviews)
        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return GeneralResponse<object>.Failure($"not found product with id: {id}");
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var result = new
            {
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Qty,
                CategoryName = product.Category.Name,
                MainImageUrl = string.IsNullOrEmpty(product.MainImageUrl) ? null : $"{baseUrl}{product.MainImageUrl}",

         
                Gallery = product.ProductImages.Select(pi => new {
                    pi.Id,
                    ImageUrl = $"{baseUrl}{pi.ImageUrl}"
                }).ToList(),
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                ReviewsCount = product.Reviews.Count()
            };

            return GeneralResponse<object>.Success(result, "Product retrieved successfully");

        }
        public async Task<GeneralResponse<object>> Delete(int id)
        {
            var product = await _context.Products
       .Include(p => p.Category)
       .Include(p => p.ProductImages)
       .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return GeneralResponse<object>.Failure($"not found product with id: {id}");
            if(product.ProductImages!=null && product.ProductImages.Any())
            {
                foreach (var image in product.ProductImages)
                {
                    ImageHelper.DeleteImage(image.ImageUrl);
                }
            }
            if (product.MainImageUrl != null) ImageHelper.DeleteImage(product.MainImageUrl);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success("Product Deleted Successfully");
        }

        public async Task<GeneralResponse<object>> Update(int id, ProductDto dto)
        {
            var product = await _context.Products
      .Include(p => p.Category)
      .Include(p => p.ProductImages)
      .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return GeneralResponse<object>.Failure($"not found product with id: {id}");
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return GeneralResponse<object>.Failure("Not valid category id");
            _mapper.Map(dto, product);
            if (dto.MainImageUrl != null)
            {
                if (product.MainImageUrl!=null) ImageHelper.DeleteImage(product.MainImageUrl);
                var newMainImage =await ImageHelper.UploadImage(dto.MainImageUrl, "products");
                product.MainImageUrl = newMainImage;
            }
            if(dto.Images !=null && dto.Images.Any())
            {
                foreach (var image in dto.Images)
                {
                    var newImage = await ImageHelper.UploadImage(image,"products");
                    product.ProductImages.Add(new ProductImage { ImageUrl = newImage });
                }
            }
            
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success(product.Id,"Product Updated successfully");
        }
        public async Task<GeneralResponse<object>> DeleteGalleryImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);

            if (image == null)
                return GeneralResponse<object>.Failure("Image not found");

            
            ImageHelper.DeleteImage(image.ImageUrl);

         
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return GeneralResponse<object>.Success("Image removed from gallery");
        }
    }
}
