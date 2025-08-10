using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;

namespace InventoryManagement.Services
{
    /// <summary>
    /// Service class responsible for managing order-related business logic.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ILoggerService<OrderService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="orderRepository">Repository to manage order data.</param>
        /// <param name="inventoryService">Service to update inventory after order operations.</param>
        /// <param name="logger">Logger for exception handling.</param>
        public OrderService(IOrderRepository orderRepository, IInventoryService inventoryService, ILoggerService<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all orders from the database.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves a specific order by its ID, including order items.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The order with its items if found; otherwise, null.</returns>
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _orderRepository.GetOrderWithItemsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Creates a new order and updates the inventory accordingly.
        /// </summary>
        /// <param name="order">The order to create.</param>
        /// <returns>The created order object.</returns>
        public async Task<Order> CreateOrderAsync(OrderCreateDTO order)
        {
            try
            {
                Order newOrder = new Order();
                newOrder.CustomerName = order.CustomerName;
                newOrder.CustomerAddress = order.CustomerAddress;
                newOrder.CustomerPhone = order.CustomerPhone;
                newOrder.Status = order.Status;
                newOrder.OrderNumber = GenerateOrderNumber();
                newOrder.CreatedDate = DateTime.UtcNow;
                newOrder.UpdatedDate = DateTime.UtcNow;
                newOrder.OrderDate = DateTime.UtcNow;

                // Calculate total amount
                newOrder.TotalAmount = newOrder.OrderItems.Sum(oi => oi.TotalPrice);

                var createdOrder = await _orderRepository.AddAsync(newOrder);

                // Update inventory
                foreach (var item in newOrder.OrderItems)
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

        /// <summary>
        /// Updates an existing order's customer and status information.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="order">The updated order data.</param>
        /// <returns>The updated order if found; otherwise, null.</returns>
        public async Task<Order> UpdateOrderAsync(int id, OrderUpdateDTO order)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                return await _orderRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves all orders along with their associated items.
        /// </summary>
        /// <returns>A list of orders with order items.</returns>
        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
        {
            try
            {
                return await _orderRepository.GetOrdersWithItemsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves an order based on its unique order number.
        /// </summary>
        /// <param name="orderNumber">The unique order number.</param>
        /// <returns>The order if found; otherwise, null.</returns>
        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            try
            {
                return await _orderRepository.GetOrderByNumberAsync(orderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves all orders filtered by a specific status.
        /// </summary>
        /// <param name="status">The order status to filter by.</param>
        /// <returns>A list of orders matching the status.</returns>
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                return await _orderRepository.GetOrdersByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Updates the status of a specific order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="status">The new order status.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
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

        /// <summary>
        /// Generates a unique order number using date and GUID.
        /// </summary>
        /// <returns>A unique string representing the order number.</returns>
        private string GenerateOrderNumber()
        {
            try
            {
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
