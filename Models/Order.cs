using System.ComponentModel.DataAnnotations;

namespace eShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Range(0.01, double.MaxValue)]
        public double TotalPrice { get; set; }
        public  string ShippingAddress { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

namespace eShop
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }
}