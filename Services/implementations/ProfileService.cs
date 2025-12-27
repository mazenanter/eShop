using AutoMapper;
using eShop.Data;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.implementations
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
        }



        public async Task<GeneralResponse<object>> GetUserInfo()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return GeneralResponse<object>.Failure("User not found please login");
            var user = await _context.Users
    .AsNoTracking()
    .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return GeneralResponse<object>.Failure("User not found please login");
            var userInfo = _mapper.Map<UserProfileDto>(user);

            return GeneralResponse<object>.Success(userInfo,"User Info Retrieved successfully");
        }

        public async Task<GeneralResponse<object>> UpdateUserInfo(UpdateUserDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return GeneralResponse<object>.Failure("User not found please login");
            var userExist = await _context.Users.AnyAsync(x => x.UserName == dto.UserName && x.Id != userId);
            if (userExist) return GeneralResponse<object>.Failure("UserName Already taken");
            _mapper.Map(dto, user);
            
            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success("User Updated Successfully");
        }
        public async Task<GeneralResponse<object>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return GeneralResponse<object>.Failure("User not found");
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return GeneralResponse<object>.Failure("Failed to change password", errors);
            }

            return GeneralResponse<object>.Success(null, "Password changed successfully");

        }

      
    }
}
