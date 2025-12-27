using System.ComponentModel.DataAnnotations;

namespace eShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Range(0, int.MaxValue)]
        public int Qty { get; set; }
    }
}
