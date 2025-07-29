using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;

        public OrderService(IOrderRepository orderRepository, IInventoryService inventoryService)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderWithItemsAsync(id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.OrderNumber = GenerateOrderNumber();
            order.CreatedDate = DateTime.UtcNow;
            order.UpdatedDate = DateTime.UtcNow;
            order.OrderDate = DateTime.UtcNow;

            // Calculate total amount
            order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice);

            var createdOrder = await _orderRepository.AddAsync(order);

            // Update inventory
            foreach (var item in order.OrderItems)
            {
                await _inventoryService.UpdateStockAsync(item.ProductId, -item.Quantity);
            }

            return createdOrder;
        }

        public async Task<Order> UpdateOrderAsync(int id, Order order)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null) return null;

            existingOrder.CustomerName = order.CustomerName;
            existingOrder.CustomerAddress = order.CustomerAddress;
            existingOrder.CustomerPhone = order.CustomerPhone;
            existingOrder.Status = order.Status;
            existingOrder.UpdatedDate = DateTime.UtcNow;

            return await _orderRepository.UpdateAsync(existingOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
        {
            return await _orderRepository.GetOrdersWithItemsAsync();
        }

        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            return await _orderRepository.GetOrderByNumberAsync(orderNumber);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status);
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = status;
            order.UpdatedDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
