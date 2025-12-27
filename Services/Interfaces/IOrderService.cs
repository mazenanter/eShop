namespace eShop.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<GeneralResponse<object>> PlaceOrder(OrderDto dto);
        public Task<GeneralResponse<object>> GetAllOrdersByUserId();
        public Task<GeneralResponse<object>> GetAllOrders();
        public Task<GeneralResponse<object>> UpdateOrderStatus(UpdateOrderStatusDto dto);
        public Task<GeneralResponse<object>> GetDashboardStats();
    }
}
