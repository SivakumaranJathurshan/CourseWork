using InventoryManagement.Models;
using InventoryManagement.Models.DTO;

namespace InventoryManagement.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(OrderCreateDTO order);
        Task<Order> UpdateOrderAsync(int id, OrderUpdateDTO order);
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<Order> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status);
    }
}
