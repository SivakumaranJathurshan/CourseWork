using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<Order> GetOrderWithItemsAsync(int id);
        Task<Order> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    }
}
