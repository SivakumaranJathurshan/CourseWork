using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersWithItemsAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("number/{orderNumber}")]
        public async Task<ActionResult<Order>> GetOrderByNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdOrder = await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> UpdateOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
            if (updatedOrder == null)
                return NotFound();

            return Ok(updatedOrder);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!result)
                return NotFound();

            return Ok(new { message = "Order status updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
