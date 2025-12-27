using Microsoft.AspNetCore.Identity;

namespace eShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpiration { get; set; }
 
        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
