using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        /// <summary>
        /// Converts the current user's cart into a formal order.
        /// </summary>
        /// <remarks>This process uses Database Transactions to ensure stock is deducted correctly.</remarks>
        [HttpPost]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));



            }
            var result = await _orderService.PlaceOrder(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpGet("userID")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetByUserId()
        {
            var result = await _orderService.GetAllOrdersByUserId();
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrders();
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                         .ToList();
                return BadRequest(GeneralResponse<object>.Failure("Validation failed", errors));



            }
            var result = await _orderService.UpdateOrderStatus(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
        /// <summary>
        /// [Admin Only] Retrieves global statistics for the dashboard.
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _orderService.GetDashboardStats();
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(200, result);
        }
    }
}
