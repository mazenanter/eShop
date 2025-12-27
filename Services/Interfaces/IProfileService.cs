namespace eShop.Services.Interfaces
{
    public interface IProfileService
    {
        public Task<GeneralResponse<object>> GetUserInfo();
        public Task<GeneralResponse<object>> UpdateUserInfo(UpdateUserDto dto);
        public Task<GeneralResponse<object>> ChangePasswordAsync(ChangePasswordDto dto);
    }
}
