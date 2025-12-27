using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<CartItem>CartItems{ get; set; }
    }
}
