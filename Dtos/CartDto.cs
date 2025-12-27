namespace eShop.Dtos
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public double GrandTotal => Items.Sum(i => i.TotalPrice);
    }
}
