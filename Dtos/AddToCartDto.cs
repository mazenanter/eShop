using System.ComponentModel.DataAnnotations;

namespace eShop.Dtos
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Qty { get; set; }
    }
}
