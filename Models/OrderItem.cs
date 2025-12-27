using System.ComponentModel.DataAnnotations;

namespace eShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Qty { get; set; }

    }
}
