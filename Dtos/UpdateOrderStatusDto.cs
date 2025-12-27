namespace eShop.Dtos
{
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
