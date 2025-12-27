using System.ComponentModel.DataAnnotations;

namespace eShop.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="UserName must be added")]
        [MinLength(2)]
        [MaxLength(200)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Password ensure if it is valid password")]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="The Phone Number is required please add it.")]


        [Phone]

        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        
    }
}
