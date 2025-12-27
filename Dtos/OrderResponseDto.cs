namespace eShop.Dtos
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public double TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponseDto> Products { get; set; }
    }
}
