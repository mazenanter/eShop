namespace eShop.Dtos
{
    public class GetOrdersDto
    {
        public DateTime CreatedAt { get; set; }
        public double TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
      
    }
}
