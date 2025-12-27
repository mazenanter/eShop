namespace eShop.Dtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double TotalPrice => Price * Qty;
    }
}
