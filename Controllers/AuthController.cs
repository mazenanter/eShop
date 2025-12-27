using eShop.Common;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed",errors));

                
                
            }

            
            var result = await _authService.RegisterAsync(registerDto);
           
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return StatusCode(201, result);

        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyEmail(VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _authService.VerifyOtpAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _authService.Login(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto dto)
        {
            if(dto.Token == null)
            {
                return BadRequest(GeneralResponse<object>.Failure("Please sent refresh token"));
            }
            var result = await _authService.RefreshTokenAsync(dto.Token);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
                    
            }
            return StatusCode(200, result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        {
            var token = dto.Token ?? Request.Headers["RefreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(GeneralResponse<string>.Failure("Token is required"));

            var result = await _authService.RevokeTokenAsync(token);

            if (!result)
                return BadRequest(GeneralResponse<string>.Failure("Invalid or already revoked token"));

            return Ok(GeneralResponse<string>.Success( "Token revoked successfully"));
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _authService.ForgotPassword(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));
            }
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(200, result);
        }
    }
}
