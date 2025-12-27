
namespace eShop.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<GeneralResponse<object>> RegisterAsync(RegisterDto dto);
        public Task<GeneralResponse<object>> VerifyOtpAsync(VerifyOtpDto dto);
        public Task<GeneralResponse<object>> Login(LoginDto dto);

        public Task<GeneralResponse<object>> RefreshTokenAsync(string refreshToken);
        public  Task<bool> RevokeTokenAsync(string token);
        public Task<GeneralResponse<object>> ForgotPassword(ForgotPasswordDto dto);
        public Task<GeneralResponse<object>> ResetPasswordAsync(ResetPasswordDto dto);

    }
}
