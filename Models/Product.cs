using System.ComponentModel.DataAnnotations;

namespace eShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? MainImageUrl { get; set; }
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Qty { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public  ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; }
    }
}
