using AutoMapper;
using eShop.Data;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

       
        public async Task<GeneralResponse<object>> PlaceOrder(OrderDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            var userCart = await _context.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userCart == null || !userCart.CartItems.Any())
                return GeneralResponse<object>.Failure("Cart is empty");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Order order = new Order
                {
                    UserId = userId,
                    ShippingAddress = dto.Address,
                    CreatedAt = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>(),
                };
                foreach (var item in userCart.CartItems)
                {
                   
                    if (item.Product.Qty < item.Qty)
                        return GeneralResponse<object>.Failure($"Product {item.Product.Name} is out of stock.");

                    order.OrderItems.Add(new OrderItem
                        {
                           
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            Price = item.Product.Price,

                        });
                        order.TotalPrice += (item.Product.Price * item.Qty);
                        item.Product.Qty -= item.Qty;
                    
                }
                await _context.Orders.AddAsync(order);
                _context.CartItems.RemoveRange(userCart.CartItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 

                return GeneralResponse<object>.Success(order.Id, "Order Placed Successfully");
            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync(); 
                return GeneralResponse<object>.Failure($"An error occurred while placing the order");
            }
           
          
            

        }

        public async Task<GeneralResponse<object>> GetAllOrdersByUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(p => p.Product)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt) 
                    .ToListAsync(); 
            if (orders == null || !orders.Any())
                return GeneralResponse<object>.Success("You haven't placed any orders yet.");
            var response = _mapper.Map<List<GetOrdersDto>>(orders);
            return GeneralResponse<object>.Success(response, "Orders retrieved successfully");
        }

        public async Task<GeneralResponse<object>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(x=>x.OrderItems).ThenInclude(oi => oi.Product)
         .OrderByDescending(x => x.CreatedAt) 
         .ToListAsync();
            if (orders == null || !orders.Any())
                return GeneralResponse<object>.Success("No Any Orders");

            var response=  _mapper.Map<List<OrderResponseDto>>(orders);
            return GeneralResponse<object>.Success(
                response
                , "Orders retrieved successfully");
        }

        public async Task<GeneralResponse<object>> UpdateOrderStatus(UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .FirstOrDefaultAsync(o => o.Id == dto.OrderId);
            if (order == null)
                return GeneralResponse<object>.Success("Not found order");
            if (order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Delivered)
            {
                return GeneralResponse<object>.Failure($"Cannot update status. Order is already {order.OrderStatus}");
            }
            if (dto.NewStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.Qty += item.Qty;
                    }
                }
            }
            order.OrderStatus = dto.NewStatus;

            await _context.SaveChangesAsync();
            return GeneralResponse<object>.Success(null, $"Order status changed to {dto.NewStatus} successfully");
        }

        public async Task<GeneralResponse<object>> GetDashboardStats()
        {
            var stats = new
            {
                totalRevenue = await _context.Orders.Where(o => o.OrderStatus != OrderStatus.Cancelled).SumAsync(x => x.TotalPrice),
                orderCount = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders.CountAsync(o => o.OrderStatus == OrderStatus.Pending),
                UsersCount = await _context.Users.CountAsync(),
                TopSellingProducts = await _context.OrderItems
            .GroupBy(oi => oi.Product.Name)
            .Select(g => new { ProductName = g.Key, Qty = g.Sum(oi => oi.Qty) })
            .OrderByDescending(x => x.Qty)
            .Take(5).ToListAsync()


            };
            return GeneralResponse<object>.Success(stats, "Stats retrieved successfully");
        }
    }
}
