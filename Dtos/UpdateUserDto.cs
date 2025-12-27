using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Dtos
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
