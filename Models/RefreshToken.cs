namespace eShop.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresON { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresON;
        public DateTime? RevokedON { get; set; }

        public bool IsActive => RevokedON == null && !IsExpired;
    }
}
