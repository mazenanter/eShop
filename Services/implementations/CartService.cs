using AutoMapper;
using eShop.Data;
using eShop.Models;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
     
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GeneralResponse<object>> AddToCartAsync(AddToCartDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return GeneralResponse<object>.Failure("User not identified.");
            var cart = await _context.Carts
        .Include(c => c.CartItems)
        .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
               
                cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                await _context.Carts.AddAsync(cart);
            }
            if (cart != null)
            {
                var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == dto.ProductId);
                if (existingItem != null)
                {
                   
                    existingItem.Qty += dto.Qty;
                    await _context.SaveChangesAsync();
                    return GeneralResponse<object>.Success("Product Quantity updated successfully");
                }
                else
                {
                
                    cart.CartItems.Add(new CartItem { ProductId = dto.ProductId, Qty = dto.Qty });
                }

            }
            await _context.SaveChangesAsync();

            return GeneralResponse<object>.Success("Cart updated successfully");


        }

        

        public async Task<GeneralResponse<object>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if(string.IsNullOrEmpty(userId))
                return GeneralResponse<object>.Failure("User not identified.");
            var cart = await _context.Carts.Include(c => c.CartItems)
                .ThenInclude(ci=>ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null || !cart.CartItems.Any())
                return GeneralResponse<object>.Success( new List<object>(), "Cart is empty");
            var response = _mapper.Map<CartDto>(cart);
            return GeneralResponse<object>.Success(response, "Cart Retrieved Successfully");
        }
        public async Task<GeneralResponse<object>> DeleteItem(int productId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return GeneralResponse<object>.Failure("User not identified.");
            var cart = await _context.Carts
        .Include(c => c.CartItems) 
        .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null || !cart.CartItems.Any())
                return GeneralResponse<object>.Success(new List<object>(), "Cart is empty");
            var product = cart.CartItems.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return GeneralResponse<object>.Failure("Product not found in your cart");
            _context.CartItems.Remove(product);
           await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success("Product Deleted Successfully");
        }

        public async Task<GeneralResponse<object>> UpdateProductQty(UpdateCartDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return GeneralResponse<object>.Failure("User not identified.");
            var cart = await _context.Carts
        .Include(c => c.CartItems)
        .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null || !cart.CartItems.Any())
                return GeneralResponse<object>.Success(new List<object>(), "Cart is empty");
            var product = cart.CartItems.FirstOrDefault(p => p.ProductId == dto.ProductId);
            if (product == null)
                return GeneralResponse<object>.Failure("Product not found in your cart");
            if(dto.Qty <= 0)
            {
                await DeleteItem(dto.ProductId);
                return GeneralResponse<object>.Success("Product Deleted Successfully");
            }
            product.Qty = dto.Qty;
      
            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success("Quantity Updated successfully");
        }
    }
}
