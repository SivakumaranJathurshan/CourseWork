using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;
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
        private readonly ILoggerService<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IInventoryService inventoryService, ILoggerService<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try { 
                return await _orderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            try { 
                return await _orderRepository.GetOrderWithItemsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try { 
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
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<Order> UpdateOrderAsync(int id, Order order)
        {
            try { 
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null) return null;

                existingOrder.CustomerName = order.CustomerName;
                existingOrder.CustomerAddress = order.CustomerAddress;
                existingOrder.CustomerPhone = order.CustomerPhone;
                existingOrder.Status = order.Status;
                existingOrder.UpdatedDate = DateTime.UtcNow;

                return await _orderRepository.UpdateAsync(existingOrder);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try { 
                return await _orderRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
        {
            try { 
                return await _orderRepository.GetOrdersWithItemsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            try { 
                return await _orderRepository.GetOrderByNumberAsync(orderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try { 
                return await _orderRepository.GetOrdersByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null) return false;

                order.Status = status;
                order.UpdatedDate = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(order);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        private string GenerateOrderNumber()
        {
            try { 
                return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }
    }
}
