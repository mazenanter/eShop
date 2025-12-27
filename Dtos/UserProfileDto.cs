namespace eShop.Dtos
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
