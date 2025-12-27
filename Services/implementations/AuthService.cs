using AutoMapper;
using eShop.Common;
using eShop.Helpers;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eShop.Services.implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWTHelper> jwtHelper,IEmailService emailService,IMapper mapper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper.Value;
            _emailService = emailService;
            _mapper = mapper;
        }

       

        public async Task<GeneralResponse<object>> RegisterAsync(RegisterDto dto)
        {
            var ExistingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (ExistingUser is not null)
            {
                if(!ExistingUser.EmailConfirmed && ExistingUser.OTPExpiration < DateTime.UtcNow)
                {
                  await  _userManager.DeleteAsync(ExistingUser);
                }
                else if (!ExistingUser.EmailConfirmed && ExistingUser.OTPExpiration > DateTime.UtcNow)
                {
                    return GeneralResponse<object>.Failure("This email is pending activation. Please check your inbox or wait for OTP to expire.");
                }else
                {
                    return GeneralResponse<object>.Failure("Email Is Already Used Try another email");
                }
            }
            if(await _userManager.FindByNameAsync(dto.UserName)is not null)
            {
                return GeneralResponse<object>.Failure("UserName Is Already Used Try another UserName");

            }
            ApplicationUser appUser = _mapper.Map<ApplicationUser>(dto);
             appUser.EmailConfirmed = false;
            var result = await _userManager.CreateAsync(appUser, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, "User");
                var OTPCode = new Random().Next(100000, 999999).ToString();
                appUser.OTP = OTPCode;
                appUser.OTPExpiration = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(appUser);
                try
                {
                    string emailBody = $"<div style='font-family:Arial;'><h2>Welcome</h2><p>Your OTP is: <b>{OTPCode}</b></p></div>";
                    await _emailService.SendEmailAsync(appUser.Email, "Confirm Your Email", emailBody);
                }
                catch (Exception ex)
                {
                    await _userManager.DeleteAsync(appUser);
                    return GeneralResponse<object>.Failure("User created but failed to send OTP. Please try to register again.");
                }
                return GeneralResponse<object>.Success(null, "Registration successful. Please check your email for the OTP.");
            }
            return GeneralResponse<object>.Failure("Error", result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<GeneralResponse<object>> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                return GeneralResponse<object>.Failure("Not found user with this email");
            }
            if (user.EmailConfirmed)
            {
                return GeneralResponse<object>.Failure("this email is already confirmed");
            }
            if (user.OTP != dto.Otp)
                return GeneralResponse<object>.Failure("Invalid OTP code.");
            if (user.OTPExpiration < DateTime.UtcNow)
                return GeneralResponse<object>.Failure("OTP has expired. Please register again or request a new one.");
            user.EmailConfirmed = true;
            user.OTP = null;
            user.OTPExpiration = null;
           var result= await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            return GeneralResponse<object>.Success(null, "Email confirmed successfully. You can now login.");
            return GeneralResponse<object>.Failure("An error occurred during verification.");
        }
        public async Task<GeneralResponse<object>> Login(LoginDto dto)
        {
            var appUser = await _userManager.FindByEmailAsync(dto.Email);
            if(appUser is null || await _userManager.CheckPasswordAsync(appUser, dto.Password) == false)
            {
                return GeneralResponse<object>.Failure("Email OR password is invalid");
            }
            if (!appUser.EmailConfirmed)
            {
                return GeneralResponse<object>.Failure("Please confirm your email first.");
            }
            AuthResponseDto authResponse = _mapper.Map<AuthResponseDto>(appUser);
            var roleList = await _userManager.GetRolesAsync(appUser);
            var token = await CreateJwt(appUser);
            authResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authResponse.Roles = roleList.ToList();
            authResponse.ExpiresOn = token.ValidTo;
            appUser.RefreshTokens ??= new List<RefreshToken>();
            var activeRefreshToken = appUser.RefreshTokens.FirstOrDefault(i => i.IsActive);
            if (activeRefreshToken != null)
            {
                authResponse.RefreshToken = activeRefreshToken.Token;
            }
            else
            {
                var refreshToken = await GenerateRefreshToken();
                authResponse.RefreshToken = refreshToken.Token;
                appUser.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(appUser);
            }
            return GeneralResponse<object>.Success(authResponse, "Login Successfully");

        }
        public async Task<GeneralResponse<object>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
        .Include(u => u.RefreshTokens)
        .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            if (user == null)
            {
                return GeneralResponse<object>.Failure("Invalid Token");
            }
            var rT = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!rT.IsActive)
            {
                return GeneralResponse<object>.Failure("Token is inactive (expired or revoked)");

            }
            AuthResponseDto authResponse = _mapper.Map<AuthResponseDto>(user);
            rT.RevokedON = DateTime.UtcNow;
            var newRefreshToken =await GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            var roleList = await _userManager.GetRolesAsync(user);
            await _userManager.UpdateAsync(user);
            var jwt = await CreateJwt(user);
            authResponse.RefreshToken = newRefreshToken.Token;
            authResponse.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            authResponse.Roles = roleList.ToList();
            authResponse.ExpiresOn = jwt.ValidTo;
            return GeneralResponse<object>.Success(authResponse, "Token refreshed successfully");
        }
        private async Task<JwtSecurityToken> CreateJwt(ApplicationUser appUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(appUser);
            var roles = await _userManager.GetRolesAsync(appUser);
            var rolesClaims = new List<Claim>();
            foreach (var roleName in roles)
            {
                rolesClaims.Add(new Claim(ClaimTypes.Role, roleName));
            }
            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub,appUser.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email,appUser.Email),
            new Claim("uid",appUser.Id),
            }.Union(userClaims).Union(rolesClaims);
            var symmanticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtHelper.Key));
            var signingCredential = new SigningCredentials(symmanticSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtHelper.Issuer,
                audience:_jwtHelper.Audience,
                signingCredentials: signingCredential,
                claims: claims,
                expires:DateTime.UtcNow.AddMinutes(_jwtHelper.DurationInMinutes)
                );

            return jwtSecurityToken;
        }
        private async Task<RefreshToken> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresON = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow
            };
        }
        public async Task<bool> RevokeTokenAsync(string token)
        {
            
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return false; 

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive) return false;

            
            refreshToken.RevokedON = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
        public async Task<GeneralResponse<object>> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                return GeneralResponse<object>.Failure("User not found with this email");
            }
            var OTPCode = new Random().Next(100000, 999999).ToString();
            user.OTP = OTPCode;
            user.OTPExpiration = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);
            try
            {
                string emailBody = $"<div style='font-family:Arial;'><h2>Welcome</h2><p>Your OTP is: <b>{OTPCode}</b></p></div>";
                await _emailService.SendEmailAsync(user.Email, "Reset Your Password", emailBody);
            }
            catch (Exception ex)
            {

                return GeneralResponse<object>.Failure("Failed to send reset code. Please try again later.");
            }
            return GeneralResponse<object>.Success(null, "Check your email for the reset code");
        }
        public async Task<GeneralResponse<object>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null || dto.OTP !=user.OTP||user.OTPExpiration < DateTime.UtcNow)
            {
                return GeneralResponse<object>.Failure("Invalid or expired OTP");
            }
      var removeResult=    await  _userManager.RemovePasswordAsync(user);
            if (removeResult.Succeeded)
            {
                await _userManager.AddPasswordAsync(user, dto.NewPassword);
                user.OTP = null; 
                await _userManager.UpdateAsync(user);
                return GeneralResponse<object>.Success(null, "Password reset successfully");
            }
            return GeneralResponse<object>.Failure("Error resetting password");
        }
    }
}
