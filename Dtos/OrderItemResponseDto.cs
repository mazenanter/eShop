namespace eShop.Dtos
{
    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}
