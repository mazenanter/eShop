
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> GetUserProfileInfo()
        {
            var result = await _profileService.GetUserInfo();
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));



            }

            var result = await _profileService.UpdateUserInfo(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpPut("change-password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));



            }

            var result = await _profileService.ChangePasswordAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
    }
}
